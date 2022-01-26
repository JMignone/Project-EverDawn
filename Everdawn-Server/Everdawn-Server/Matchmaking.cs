using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everdawn_Server
{
    class Matchmaking
    {
        public Matchmaking()
        {
            // Get/Set player ELO
            // Calculate winner ELO
            //      Math.round((winnerELO + (winnerK * (1 - (1 / (Math.pow(10, (loserELO - winnerELO) / 850) + 1))))) * 1000) / 1000

            // Calculate loser ELO
            //      Math.round((loserELO - (loserK * (1 / (Math.pow(10, (winnerELO - loserELO) / 850) + 1)))) * 1000) / 1000

            // Calculate tied ELO
            //      Higher ELO:
            //          Math.round((HigherELO + (winnerK * (0.5 - 1 / (1 + Math.pow(10, (LowerELO - HigherELO) / 850))))) * 1000) / 1000

            //      Lower ELO: <= PREFERRED
            //          Math.round((LowerELO + (loserK * (0.5 - 1 / (1 + Math.pow(10, (HigherELO - LowerELO) / 850))))) * 1000) / 1000

            //TODO Implement RankLock system checks
        }
    }
}
