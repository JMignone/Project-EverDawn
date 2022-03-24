using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputAnimator : MonoBehaviour //IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool playTapAnimation;
    [SerializeField] private bool playCursorTrail;
    [SerializeField] private ParticleSystem tapAnimationParticleSystem;
    [SerializeField] private ParticleSystem cursorTrailParticleSystem;
    [SerializeField] private GameObject instantiationObject;
    private ParticleSystem cursorTrailInstance;
    private Vector2 inputPosition;

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playTapAnimation) // Check if tap animation is enabled
        {
            Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 1);
            Instantiate(tapAnimationParticleSystem, pos, Quaternion.identity);
        }
    }
    */


    // Update is called once per frame
    void Update()
    {
        inputPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // Set inputPosition variable to be the location of the click
        transform.position = inputPosition; // Change Instantiation location to be the same as the input position
        if (playTapAnimation == true && Input.GetMouseButtonDown(0) == true) // Check if tap animation is enabled and if a tap/click has occurred
        {
            TapAnimationPlay(); // Play tap animation
        }
        if (playCursorTrail == true && cursorTrailInstance == null && Input.GetMouseButtonDown(0) == true) // Check if cursor trail is enabled, if there is an exiting cursor trail, and if a tap/click has occurred
        {
            CursorTrailPlay();
        }
        if(Input.GetMouseButtonDown(0) == false && cursorTrailInstance != null)
        {
            GameObject.Destroy(cursorTrailInstance);
        }
    }

    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(playCursorTrail == true) // Check if cursor trail is enabled
        {
            Debug.Log("BeginDrag");
            CursorTrailPlay();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        GameObject.Destroy(cursorTrailInstance);
    }
    */

    // Play the tap animation particle system at input location
    public void TapAnimationPlay()
    {
        //Intatntiate the TapAnimation particle system
        Instantiate(tapAnimationParticleSystem, inputPosition, Quaternion.identity);
    }

    // Play the cursor trail particle system at input location
    public void CursorTrailPlay()
    {
        cursorTrailInstance = Instantiate(cursorTrailParticleSystem, instantiationObject.transform); //Intatntiate the cursor trail particle system as cursorTrailInstance
    }
    

} // End of Class
