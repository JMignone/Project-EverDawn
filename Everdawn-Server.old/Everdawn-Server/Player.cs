using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everdawn_Server
{
    public class Player
    {
        public Guid UUID { get; set; }
        // Get/Set player casual rank
        public int playerCasualRank { get; set; }
        // Get/Set player competitive rank
        public int playerCompetitiveRank { get; set; }
        // Get/Set player total games played
        public int totalGamesPlayed { get; set; }
        // Get/Set player total games won
        public int totalGamesWon { get; set; }
        // Get/Set player total games lost
        public int totalGamesLost { get; set; }
        // Get/Set player total games tied
        public int totalGamesTied { get; set; }
        // Get/Set player win streak
        public int currentWinStreak { get; set; }
        // Get/Set player highest win streak
        public int highestWinStreak { get; set; }
        // Get/Set player lose streak
        public int currentLoseStreak { get; set; }
        // Get/Set player highest lose streak
        public int highestLoseStreak { get; set; }
        // Get/Set player tie streak
        public int currentTieStreak { get; set; }
        // Get/Set player highest tie streak
        public int highestTieStreak { get; set; }

        public static Player Create(IDataRecord record)
        {
            return new Player
            {
                UUID =                  (Guid)record["UUID"],
                playerCasualRank =      (int)record["playerCasualRank"],
                playerCompetitiveRank = (int)record["playerCompetitiveRank"],
                totalGamesPlayed =      (int)record["totalGamesPlayed"],
                totalGamesWon =         (int)record["totalGamesWon"],
                totalGamesLost =        (int)record["totalGamesLost"],
                totalGamesTied =        (int)record["totalGamesTied"],
                currentWinStreak =      (int)record["currentWinStreak"],
                highestWinStreak =      (int)record["highestWinStreak"],
                currentLoseStreak =     (int)record["currentLoseStreak"],
                highestLoseStreak =     (int)record["highestLoseStreak"],
                currentTieStreak =      (int)record["currentTieStreak"],
                highestTieStreak =      (int)record["highestTieStreak"]
            };
        }
        public IEnumerable<Player> GetPlayers()
        {
            yield return Player.Create((IDataRecord)PGSQL.query("SELECT * FROM playerProfiles").AsEnumerable());
        }
        public Player GetPlayer(Guid GUID)
        {
            return Player.Create((IDataRecord)PGSQL.query($"Select * FROM playerProfiles WHERE GUID={GUID}"));
        }
    }
}
