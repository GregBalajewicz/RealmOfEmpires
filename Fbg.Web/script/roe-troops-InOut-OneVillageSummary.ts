module ROE {


    export class InOutOneVillageSummary {
        public village: VillageStruct;
        public numIncomingAttack: number = 0;
        public numIncomingAttackHidden: number = 0;
        public numIncomingOther: number = 0;
        public numIncomingOtherHidden: number = 0;
        public earliestLandingAttack: number = Number.MAX_VALUE;
        public earliestLandingAttackHidden: number = Number.MAX_VALUE;
        public earliestLandingOther: number = Number.MAX_VALUE;
        public earliestLandingOtherHidden: number = Number.MAX_VALUE;
        public mostSignificantIncoming: MostSignificantIncoming;



        constructor() {
            this.village = new VillageStruct();
        }

        public getMostSignificantIncoming() {

            if (!this.mostSignificantIncoming) {
                this.mostSignificantIncoming = new MostSignificantIncoming();
                if (this.numIncomingAttack > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingAttack;
                    this.mostSignificantIncoming.type = MostSignificantIncomingType.Attack;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingAttack;
                }
                else if (this.numIncomingAttackHidden > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingAttackHidden;
                    this.mostSignificantIncoming.type = MostSignificantIncomingType.AttackHidden;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingAttackHidden;
                }
                else if (this.numIncomingOther > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingOther;
                    this.mostSignificantIncoming.type = MostSignificantIncomingType.Support;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingOther;
                }
                else if (this.numIncomingOtherHidden > 0) {
                    this.mostSignificantIncoming.num = this.numIncomingOtherHidden;
                    this.mostSignificantIncoming.type = MostSignificantIncomingType.SupportHidden;
                    this.mostSignificantIncoming.earliestLanding = this.earliestLandingOtherHidden;
                }
            }
            return this.mostSignificantIncoming;
        }

    }

    export class VillageStruct {
        public ownerPID: number;
        public ownerName: string;
        public name: string;
        public x: number;
        public y: number;
        public id: number;
    }



    export class MostSignificantIncoming {
        public num: number;
        public earliestLanding: number;
                public type: MostSignificantIncomingType
            }


}

enum MostSignificantIncomingType {
    Attack,
    AttackHidden,
    Support,
    SupportHidden,
}