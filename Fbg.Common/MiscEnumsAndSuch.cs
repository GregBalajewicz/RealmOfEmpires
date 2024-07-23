using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common
{

    /// <summary>
    /// result of transporting coins - calls to transport coins
    /// </summary>
    public enum TransportResult : int
    {
        Success = 0,
        Village_Not_Found = 1,
        Coins_More_then_Allowed = 2,
        Coins_must_be_greater_then_Zero = 3,
        Only_Numbers_Accepted = 4,
        Same_Village = 5
    }

    /// <summary>
    /// for coin transports
    /// </summary>
    public enum TransportDirection : int
    {
        Transporting = 0,
        Returning = 1

    }

    public enum OfferCompanies : int
    {
        //AdParlor = 1,
        OfferPal = 2,
        //SuperRewards = 3,
        //gWallet = 4,
        LinkEx=5,
        ChartBoost=6
    }

    public enum StartInQuadrants
    {
        NoneSelected = 0,
        NorthEast = 1,
        SouthEast = 2,
        SouthWest = 3,
        NorthWest = 4
    }
    public enum BuyResearcherResult
    {
        ok = 1,
        failed_maxResearchersReached = 2,
        failed_noCredits = 3
    }


    public enum RecordPaymentTransaction_ReturnValue
    {
        /// <summary>
        /// means that we have a transaction like this already
        /// </summary>
        Failure_SuchTranAndStatusExists = 0,
        Success = 1
    }

}
