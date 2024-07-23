var ROE;
(function (ROE) {
    var InOutOneVillageSummary = (function () {
        function InOutOneVillageSummary() {
            this.numIncomingAttack = 0;
            this.numIncomingAttackHidden = 0;
            this.numIncomingOther = 0;
            this.numIncomingOtherHidden = 0;
            this.earliestLandingAttack = Number.MAX_VALUE;
            this.earliestLandingAttackHidden = Number.MAX_VALUE;
            this.earliestLandingOther = Number.MAX_VALUE;
            this.earliestLandingOtherHidden = Number.MAX_VALUE;
            this.village = new VillageStruct();
        }
        InOutOneVillageSummary.prototype.getMostSignificantIncoming = function () {
            if (!this.mostSignificantIncoming) {
                this.mostSignificantIncoming = new MostSignificantIncoming();
                if (this.numIncomingAttack > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingAttack;
                    this.mostSignificantIncoming.type = 0 /* Attack */;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingAttack;
                } else if (this.numIncomingAttackHidden > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingAttackHidden;
                    this.mostSignificantIncoming.type = 1 /* AttackHidden */;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingAttackHidden;
                } else if (this.numIncomingOther > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingOther;
                    this.mostSignificantIncoming.type = 2 /* Support */;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingOther;
                } else if (this.numIncomingOtherHidden > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingOtherHidden;
                    this.mostSignificantIncoming.type = 3 /* SupportHidden */;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingOtherHidden;
                }
            }
            return this.mostSignificantIncoming;
        };
        return InOutOneVillageSummary;
    })();
    ROE.InOutOneVillageSummary = InOutOneVillageSummary;

    var VillageStruct = (function () {
        function VillageStruct() {
        }
        return VillageStruct;
    })();
    ROE.VillageStruct = VillageStruct;

    var MostSignificantIncoming = (function () {
        function MostSignificantIncoming() {
        }
        return MostSignificantIncoming;
    })();
    ROE.MostSignificantIncoming = MostSignificantIncoming;
})(ROE || (ROE = {}));

var MostSignificantIncomingType;
(function (MostSignificantIncomingType) {
    MostSignificantIncomingType[MostSignificantIncomingType["Attack"] = 0] = "Attack";
    MostSignificantIncomingType[MostSignificantIncomingType["AttackHidden"] = 1] = "AttackHidden";
    MostSignificantIncomingType[MostSignificantIncomingType["Support"] = 2] = "Support";
    MostSignificantIncomingType[MostSignificantIncomingType["SupportHidden"] = 3] = "SupportHidden";
})(MostSignificantIncomingType || (MostSignificantIncomingType = {}));
