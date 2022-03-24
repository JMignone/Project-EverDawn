using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ComputerStats
{
    [SerializeField]
    private bool isComputer;

    private PlayerStats playerInfo;
    private List<CardStats> handSnapshot;

    [SerializeField]
    private int currResource;
    private List<List<int>> potentialCardCombos;
    [SerializeField]
    private bool isPlaying;
    [SerializeField]
    private List<int> order;
    private List<Card> cardOrder;
    [SerializeField]
    private int playIndex;
    private Vector3 location;
    [SerializeField]
    private float playDelay;
    [SerializeField]
    private float currentDelay;
    

    public bool IsComputer
    {
        get { return isComputer; }
    }

    public void Start(PlayerStats pInfo) {
        playerInfo = pInfo;
        currResource = pInfo.GetCurrResource;
        potentialCardCombos = new List<List<int>>();
        cardOrder = new List<Card>();
    }

    public void UpdateComputerStats() {
        if(isComputer && !GameManager.Instance.Complete) {
            if(isPlaying) {
                if(currentDelay < playDelay)
                    currentDelay += Time.deltaTime;
                else {
                    Vector3 dropLocation = adjustLocation();
                    cardOrder[playIndex].QueCard(dropLocation);
                    cardOrder[playIndex].GetNewCard();

                    currentDelay = 0;

                    playIndex++;
                    if(playIndex == order.Count) {
                        isPlaying = false;
                        cardOrder = new List<Card>();
                    }
                }
            } //if there are any 0 cost cards, there is a very small chance that a bot plays only a 0 cost card at 10 resource, causing the bot to stop playing cards
            if(playerInfo.GetCurrResource == currResource + 1 && !isPlaying) {
                currResource = playerInfo.GetCurrResource;
                //MonoBehaviour.print(currResource);
                if(playDecision()) {
                    order = playOrder();
                    //MonoBehaviour.print("cardIndexOrder: " + string.Join(",", order));
                    foreach(int index in order)
                        cardOrder.Add(playerInfo.HandParent.transform.GetChild(index).GetComponent<Card>());
                        
                    location = playLocation();
                    //MonoBehaviour.print(location);
                    playIndex = 0;
                    playDelay = (1/GameConstants.RESOURCE_SPEED)/order.Count;
                    currentDelay = (1/GameConstants.RESOURCE_SPEED * .5f)/order.Count;

                    isPlaying = true;
                }
            }
            currResource = playerInfo.GetCurrResource;
        }
    }

    private bool playDecision() {
        int lowestCost = Mathf.Min(playerInfo.PlayersDeck.Hand[0].Cost, playerInfo.PlayersDeck.Hand[1].Cost,
                                playerInfo.PlayersDeck.Hand[2].Cost, playerInfo.PlayersDeck.Hand[3].Cost);

        if(currResource < lowestCost)
            return false;

        float random = Random.Range(0.0f, 1.0f);
        float totalEnemyHP = 0;
        foreach (GameObject go in GameManager.Instance.Objects) {
            if(!go.CompareTag(playerInfo.transform.tag) && !GameManager.Instance.TowerObjects.Contains(go)) { //if the unit is an enemy and not a tower
                IDamageable unit = go.GetComponent(typeof(IDamageable)) as IDamageable;
                if(unit.Agent.transform.position.z > 0)
                    totalEnemyHP += unit.Stats.CurrHealth + unit.Stats.CurrArmor;
            }
        }
        float chance = Mathf.Pow(currResource, 2.5f + (Mathf.Pow(totalEnemyHP, 2.718f) * (GameConstants.RESOURCE_MAX + 1f - currResource))/100000000f ) 
            / Mathf.Pow(GameConstants.RESOURCE_MAX + 1, 2.5f);
        //MonoBehaviour.print(chance * 100);
        //MonoBehaviour.print(random * 100);
        return chance > random; //somtimes this returns an ArgumentOutOfRangeException, but everything continues to run normally. Why!!!
    }

    private List<int> playOrder() {
        List<int> numbers = new List<int>();
        numbers.Add(playerInfo.PlayersDeck.Hand[0].Cost);
        numbers.Add(playerInfo.PlayersDeck.Hand[1].Cost);
        numbers.Add(playerInfo.PlayersDeck.Hand[2].Cost);
        numbers.Add(playerInfo.PlayersDeck.Hand[3].Cost);
        sum_up(numbers, currResource);

        int random = (int) Random.Range(0.0f, potentialCardCombos.Count);
        potentialCardCombos[random].Sort();
        potentialCardCombos[random].Reverse();

        //MonoBehaviour.print("cardCostCombo: " + string.Join(",", potentialCardCombos[random]));
        int cost = 0;

        int[] cardIndex = new int[] {0, 1, 2, 3};
        for(int x=0; x<potentialCardCombos[random].Count; x++) {
            cost += potentialCardCombos[random][x];
            for(int index=0; index<cardIndex.Length; index++) {
                /*if(cardIndex[index] != -1 && playerInfo.PlayersDeck.Hand[index].Cost == potentialCardCombos[random][x]) {
                    potentialCardCombos[random][x] = index;
                    cardIndex[index] = -1;
                    break;
                }*/
                if(cardIndex[index] != -1 && playerInfo.HandParent.GetChild(index).GetComponent<Card>().CardInfo.Cost == potentialCardCombos[random][x]) {
                    potentialCardCombos[random][x] = index;
                    cardIndex[index] = -1;
                    break;
                }
            }
        }

        currResource -= cost;

        List<int> combos = potentialCardCombos[random];
        potentialCardCombos = new List<List<int>>();

        return combos;
    }

    private Vector3 playLocation() {
        float totalRightHp = 0;
        float totalLeftHp = 0;
        foreach (GameObject go in GameManager.Instance.Objects) { //  The trigger exit doesnt get trigger if the object suddenly dies, so we need this do do it manually
            if(!GameManager.Instance.TowerObjects.Contains(go)) { //if the unit is not a tower
                IDamageable unit = go.GetComponent(typeof(IDamageable)) as IDamageable;
                if(unit.Agent.transform.position.x > 0) { //if the unit is on the right side
                    if(unit.Agent.transform.position.z > 0 && !go.CompareTag(playerInfo.transform.tag)) //if an enemy unit is on your side, double the weight
                        totalRightHp += unit.Stats.CurrHealth + unit.Stats.CurrArmor;
                    totalRightHp += unit.Stats.CurrHealth + unit.Stats.CurrArmor;
                }
                else {
                    if(unit.Agent.transform.position.z > 0 && !go.CompareTag(playerInfo.transform.tag))
                        totalLeftHp += unit.Stats.CurrHealth + unit.Stats.CurrArmor;
                    totalLeftHp += unit.Stats.CurrHealth + unit.Stats.CurrArmor;
                }
            }
        }

        //normalize the hp values for the weighting
        float leftChance = 1;

        if(totalRightHp == 0 && totalLeftHp == 0)
            leftChance = .5f;
        if(totalRightHp == 0 && totalLeftHp != 0)
            leftChance = 1;
        else if(totalRightHp != 0 && totalLeftHp == 0)
            leftChance = 0;
        else if(totalRightHp > totalLeftHp)
            leftChance = totalLeftHp/(totalRightHp+totalLeftHp);

        float random = Random.Range(0.0f, 1.0f);
        Vector3 groundScale = GameManager.Instance.Ground.transform.localScale;

        float meanX = groundScale.x*10/4;
        float meanZ = groundScale.z*10/4;
        float standardDeviationX = meanX/2;
        float standardDeviationZ = meanZ/2;

        float u1 = Random.Range(0.0f, 1.0f); //uniform(0,1] random doubles
        float u2 = Random.Range(0.0f, 1.0f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormalX = meanX + standardDeviationX * randStdNormal; //random normal(mean,stdDev^2)

        u1 = Random.Range(0.0f, 1.0f); //uniform(0,1] random doubles
        u2 = Random.Range(0.0f, 1.0f);
        randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormalZ = meanZ + standardDeviationZ * randStdNormal; //random normal(mean,stdDev^2)

        if(randNormalX < -groundScale.x*10/2)
            randNormalX = -groundScale.x*10/2;
        else if(randNormalX > groundScale.x*10/2)
            randNormalX = groundScale.x*10/2;

        if(randNormalZ < 6.5f)
            randNormalZ = 6.5f;
        else if(randNormalZ > groundScale.z*10/2)
            randNormalZ = groundScale.z*10/2;

        if(leftChance > random)
            randNormalX *= -1;

        if(playerInfo.transform.tag == "Player")
            randNormalZ *= -1;

        return new Vector3(randNormalX, 0, randNormalZ);
    }

    private Vector3 adjustLocation() {
        CardStats cardInfo = playerInfo.HandParent.GetChild(order[playIndex]).GetComponent<Card>().CardInfo;
        Vector3 position = location;
        int navMask = 0;

        if(cardInfo.UnitIndex != -1) {
            IDamageable unit = (cardInfo.Prefab[cardInfo.UnitIndex].GetComponent(typeof(IDamageable)) as IDamageable);
            if(unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                position.y = GameConstants.FLY_ZONE_HEIGHT;

            int agentTypeID = cardInfo.PreviewPrefab.transform.GetChild(0).GetComponent<UnityEngine.AI.NavMeshAgent>().agentTypeID;
            if(unit.Stats.UnitType == GameConstants.UNIT_TYPE.STRUCTURE) {
                if(agentTypeID == 287145453) //the agent type id for big building
                    navMask = 32;
                else
                    navMask = 16;
            }
            else if(unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                navMask = 8;
            else
                navMask = 1;
        }
        Vector3 groundScale = GameManager.Instance.Ground.transform.localScale;

        float random = Random.Range(-10.0f, 10.0f);
        position.x += random;

        if(position.x < -groundScale.x*10/2)
            position.x = -groundScale.x*10/2;
        else if(position.x > groundScale.x*10/2)
            position.x = groundScale.x*10/2;

        UnityEngine.AI.NavMeshHit hit;
        position = GameFunctions.adjustForBoundary(position);

        if(cardInfo.UnitIndex != -1) {
            if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, GameConstants.SAMPLE_POSITION_RADIUS, navMask))
                position = hit.position;
        }
        position.y = 1;
        
        location.z += 10;
        if(location.z > groundScale.z*10/2)
            location.z = groundScale.z*10/2;

        return position;
    }

    //Below 2 found from https://stackoverflow.com/questions/4632322/finding-all-possible-combinations-of-numbers-to-reach-a-given-sum
    private void sum_up(List<int> numbers, int target)
    {
        sum_up_recursive(numbers, target, new List<int>());
    }

    private void sum_up_recursive(List<int> numbers, int target, List<int> partial)
    {
        int s = 0;
        foreach (int x in partial) s += x;

        if (s <= target && partial.Count != 0) {
            //MonoBehaviour.print("sum(" + string.Join(",", partial.ToArray()) + ")=" + target);
            potentialCardCombos.Add(new List<int>(partial));
        }

        if (s >= target)
            return;

        for (int i = 0; i < numbers.Count; i++)
        {
            List<int> remaining = new List<int>();
            int n = numbers[i];
            for (int j = i + 1; j < numbers.Count; j++) remaining.Add(numbers[j]);

            List<int> partial_rec = new List<int>(partial);
            partial_rec.Add(n);
            sum_up_recursive(remaining, target, partial_rec);
        }
    }

}
