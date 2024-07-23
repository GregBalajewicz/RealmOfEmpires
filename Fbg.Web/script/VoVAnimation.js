
ROE.vovAnimations = {}

ROE.vovAnimations.barracks_sign = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_barracksSignExt2.png",
    width: 24,
    height: 24,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
}

ROE.vovAnimations.barracks_sign_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_barracksSignExt2.png",
    width:24,
    height:24,
    animations: { 	
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.chickens01 = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/chickens01.png",
    width: 30,
    height: 20,
    animations: {
        first: { frames: [0, 0, 0, 1, 0], next: "two", speed: 1 },
        two: { frames: [1, 1, 2, 3, 3], next: "three", speed: 1 },
        three: { frames: [0, 0], next: "four", speed: .15 },
        four: { frames: [3, 0], next: "five", speed: 1 },
        five: { frames: [3, 3, 4, 5, 5, 0, 1, 1, 1, 0, 5, 4, 6, 2, 1, 0, 5, 4, 4, 3, 0, 0], next: "six", speed: 1 },
        six: { frames: [5, 0, 3, 0, 3], next: "seven", speed: 1 },
        seven: { frames: [5, 5, 7, 1, 1], next: "last", speed: 1 },
        last: { frames: [0, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.guyByWater = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/GuyByWaterB.png", 
    width:36,
    height: 36,
    animations: { first: [0, 5, 'first', 4] }
};

ROE.vovAnimations.stables_sign = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_stablesSignExt.png", 
    width:29,
    height:26,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.stables_sign_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_stablesSignExt.png", 
    width:29,
    height:26,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.geese = {
    spritesheet: "https://static.realmofempires.com/images/anim/geeseFlight.png",
    width: 54,
    height: 30,
    animations: { first: { frames: [0, 1, 2, 3, 4, 5], next: 'first', speed: 1 } }
};

ROE.vovAnimations.goose = {
    spritesheet: "https://static.realmofempires.com/images/anim/gooseFlight.png", 
    width:20,
    height: 10,
    animations: { first: { frames: [0, 1, 2, 3, 4, 5], next: 'first', speed: 1 } }
};

ROE.vovAnimations.barnOwlFlight_n = {
    spritesheet: "https://static.realmofempires.com/images/anim/n_barnOwlFlight.png", 
    width:20,
    height: 16,
    animations: { first: { frames: [0, 1, 2, 3, 4, 5], next: 'first', speed: 1 } }
};

ROE.vovAnimations.treasury_sign = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_treasurySignExt.png", 
    width:29,
    height:25,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.treasury_sign_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_treasurySignExt.png", 
    width:29,
    height:25,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.tavern_sign = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_tavernSignExt2.png",
    width:32,
    height:29,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};


ROE.vovAnimations.tavern_sign_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_tavernSignExt2.png",
    width: 32,
    height: 29,
    animations: {
        first: { frames: [0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 2, 5, 6, 5, 2, 5, 6, 6, 5, 2, 0, 4, 7, 7, 4, 0, 1, 2, 5, 5, 2, 1, 0, 3, 4, 4, 3, 0, 1, 2, 2, 1, 0, 3, 3, 0, 1], next: "pause", speed: 1 },
        pause: { frames: [0, 0], next: "last", speed: .2 },
        last: { frames: [2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 5, 2, 5, 6, 6, 6, 5, 5, 2, 0, 4, 7, 8, 8, 7, 4, 0, 1, 2, 2, 5, 5, 5, 2, 2, 0, 3, 4, 4, 7, 7, 4, 4, 3, 0, 1, 1, 2, 2, 2, 1, 1, 0, 3, 4, 4, 3, 0, 1, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.scarecrow = {
    wind: true,
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_scarecrow.png", 
    width:30,
    height:35,
    animations: {
        first: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0,0,0], next: "mid", speed: .5 },
        mid: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0], next: "last", speed: .5 },
        last: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0,0,0,0,0,0,0], next: "first", speed: .5 }
    }
};

ROE.vovAnimations.scarecrow_n = {
    wind: true,
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_scarecrow.png", 
    width:30,
    height:35,
    animations: {
        first: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 0, 0], next: "mid", speed: .5 },
        mid: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0], next: "last", speed: .5 },
        last: { frames: [1, 1, 0, 2, 3, 3, 2, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0], next: "first", speed: .5 }
    }
};

ROE.vovAnimations.civiliansChat = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_civiliansChatA.png",
    width: 35,
    height: 32,
    animations: {
        first: {
            frames: [0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 5, 5, 5, 5, 5, 5, 4, 4, 4, 3, 3, 3, 4, 4, 4, 3, 3, 3, 4, 4, 4, 3, 3, 3, 6, 6, 6, 5, 5, 5, 6, 6, 6, 5, 5, 5, 6, 2, 2, 7, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 6, 6, 6, 0, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0],
            next: 'first',
            speed: 1
        }
    }
};

ROE.vovAnimations.civiliansChat_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_civiliansChatA.png",
    width: 35,
    height: 32,
    animations: {
        first: {
            frames: [0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 5, 5, 5, 5, 5, 5, 4, 4, 4, 3, 3, 3, 4, 4, 4, 3, 3, 3, 4, 4, 4, 3, 3, 3, 6, 6, 6, 5, 5, 5, 6, 6, 6, 5, 5, 5, 6, 2, 2, 7, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 8, 8, 8, 0, 0, 0, 8, 8, 8, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0],
            next: 'first',
            speed: 1
        }
    }
};

//the knight horse grazing behind the tavern
ROE.vovAnimations.knightHorse = {
    pause: 1000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_knightHorse01.png",
    width: 80,
    height: 88,
    animations: {
        first: { frames: [0, 1, 2, 3], speed: .5, next: "standDown" }
            , standDown: { frames: [3, 3, 3, 3], speed: .1, next: "up" }
            , up: { frames: [3, 2, 1, 0], speed: .5, next: "last" }
            , last: { frames: [0], speed: .5, next: false }

    }
};
ROE.vovAnimations.knightHorse_n = {
    pause: 1000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_knightHorse01.png",
    width: 80,
    height: 88,
    animations: {
        first: { frames: [0, 1, 2, 3], speed: .5, next: "standDown" }
            , standDown: { frames: [3, 3, 3, 3], speed: .1, next: "up" }
            , up: { frames: [3, 2, 1, 0], speed: .5, next: "last" }
            , last: { frames: [0], speed: .5, next: false }

    }
};

ROE.vovAnimations.chat = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_soldiersChat.png", 
    width:71,
    height:71,
    animations: { 	first:{ frames:[0,1,0,1,0,1,0,1,0,1,0,1,0,1], next:"pauseOne", speed: 1},
        pauseOne:{ frames:[0,0], next:"chat", speed: .25},
        chat:{ frames:[0,1,0,1,0,1,0,1], next:"pauseTwo", speed: 1},
        pauseTwo:{ frames:[0,0], next:"last", speed: .25},
        last:{ frames:[2,3,3,3,4,3,2,0], next:"first", speed: 1}
    }
};

ROE.vovAnimations.chat_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_soldiersChat.png", 
    width:71,
    height:71,

    animations: { 	first:{ frames:[0,1,0,1,0,1,0,1,0,1,0,1,0,1], next:"pauseOne", speed: 1},
        pauseOne:{ frames:[0,0], next:"chat", speed: .25},
        chat:{ frames:[0,1,0,1,0,1,0,1], next:"pauseTwo", speed: 1},
        pauseTwo:{ frames:[0,0], next:"last", speed: .25},
        last:{ frames:[2,3,3,3,4,3,2,0], next:"first", speed: 1}
    }
};

//the guy infront of the barracks swinging his sword
ROE.vovAnimations.infantrySlash = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_infantrySlash.png",
    width: 52,
    height: 50,
    animations: {
        first: { frames: [0, 0, 0, 0, 0, 0, 0, 0], next: "swingStart", speed: .33 },
        swingStart: [0, 3, "swingTwo", 4],
        swingTwo: { frames: [4, 5, 6], next: "last" },
        last: [7, 10, "first", 1]
    }
};
ROE.vovAnimations.infantrySlash_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_infantrySlash.png", 
    width:52,
    height:50,
    animations: {
        first: { frames: [0, 0, 0, 0, 0, 0, 0, 0], next: "swingStart", speed: .33 },
        swingStart: [0, 3, "swingTwo", 4],
        swingTwo: { frames: [4, 5, 6], next: "last" },
        last: [7, 10, "first", 1]
    }
};

ROE.vovAnimations.flag01 = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_flag01_ext.png", 
    width:29,
    height:28,
    animations: { 	
        first: { frames: [0, 1, 2], next: "two", speed: .8 },
        two:{ frames:[3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7], next:"last", speed:.8},
        last: { frames: [2, 1, 0], next: "first", speed: .8 }
    }
};

ROE.vovAnimations.flag01_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_flag01_ext.png", 
    width:29,
    height:28,
    animations: { 
        first: { frames: [0, 1, 2], next: "two", speed: .8 },
        two: { frames: [3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7], next: "last", speed: .8 },
        last: { frames: [2, 1, 0], next: "first", speed: .8 }
    }
};

ROE.vovAnimations.flag02 = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_flag02_ext.png", 
    width:44,
    height:36,
    animations: { 	
        first:{ frames:[0,1,2], next:"two", speed: 1},
        two:{ frames:[3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7], next:"last", speed: 1},
        last:{ frames:[2,1,0], next:"first", speed: 1}
    }
};

ROE.vovAnimations.flag02_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_flag02_ext.png",
    width: 44,
    height: 36,
    animations: {
        first: { frames: [0, 1, 2], next: "two", speed: 1 },
        two: { frames: [3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7, 3, 4, 5, 6, 7], next: "last", speed: 1 },
        last: { frames: [2, 1, 0], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.flag03 = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/d_flag03_ext.png", 
    width:33,
    height:31,
    animations: { 	
        first:{ frames:[0,1,2], next:"two", speed: 1},
        two:{ frames:[3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7], next:"last", speed: 1},
        last:{ frames:[2,1,0], next:"first", speed: 1}
    }
};

ROE.vovAnimations.flag03_n = {
    wind: true,
    spritesheet: "https://static.realmofempires.com/images/anim/n_flag03_ext.png", 
    width:33,
    height:31,
    animations: { 	
        first:{ frames:[0,1,2], next:"two", speed: 1},
        two:{ frames:[3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7,3,4,5,6,7], next:"last", speed: 1},
        last:{ frames:[2,1,0], next:"first", speed: 1}
    }
};

ROE.vovAnimations.hay_prod = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_hayProd.png", 
    width:24,
    height:29,
    animations: {
        first: { frames: [0, 0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 2, 1, 2, 1, 0], next: "last", speed: 1 },
        last: { frames: [0, 1, 0, 1, 2, 1, 0, 1, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.hayProd_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_hayProd.png", 
    width:24,
    height:29,
    animations: {
        first: { frames: [0, 0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 2, 1, 2, 1, 0], next: "last", speed: 1 },
        last: { frames: [0, 1, 0, 1, 2, 1, 0, 1, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.horseTail = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_horseTail.png",
    width:22,
    height:17,
    animations: {
        first: { frames: [0, 1, 2, 3, 4, 5, 6, 7, 8], next: "last", speed: 1 },
        last: { frames: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.horseTail_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_horseTail.png",
    width:22,
    height:17,
    animations: {
        first: { frames: [0, 1, 2, 3, 4, 5, 6, 7, 8], next: "last", speed: 1 },
        last: { frames: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.leatherArmorGuy = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/d_leatherArmorGuy.png", 
    width:23,
    height:34,
    animations: {
        first: { frames: [0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 2, 3, 4, 5, 4, 5, 4, 5, 4, 5, 4, 5, 5, 5, 5, 4, 5, 4, 3, 2, 0, 1], next: "second", speed: .8 },
        second: { frames: [0, 0], next: "last", speed: 0.1 },
        last: { frames: [0, 1, 0, 2, 3, 4, 5, 4, 5, 4, 3, 2, 0, 1, 0, 1, 0], next: "first", speed: .8 }
    }
};

ROE.vovAnimations.leatherArmorGuy_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_leatherArmorGuy.png", 
    width:23,
    height:34,
    animations: {
        first: { frames: [0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 2, 3, 4, 5, 4, 5, 4, 5, 4, 5, 4, 5, 5, 5, 5, 4, 5, 4, 3, 2, 0, 1], next: "second", speed: .8 },
        second: { frames: [0, 0], next: "last", speed: 0.1 },
        last: { frames: [0, 1, 0, 2, 3, 4, 5, 4, 5, 4, 3, 2, 0, 1, 0, 1, 0], next: "first", speed: .8 }
    }
};

ROE.vovAnimations.bonfire_n = {
    spritesheet: "https://static.realmofempires.com/images/anim/bonfire.png",
    width: 79,    height: 74,    animations: {
        first: { frames: [0, 1, 2], next: "last", speed: 1 },
        last: { frames: [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.tavern_glow_1_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_tavern1_glow.png",
    width: 145,
    height: 157,
    animations: {
        first: { frames: [0, 1, 2], next: "last", speed: .6 },
        last: { frames: [3, 4, 5], next: "first", speed: .6 }
    }
};

ROE.vovAnimations.tavern_glow_3_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_tavern3_glow.png",
    width: 145,
    height: 157,
    animations: {
        first: { frames: [0, 1, 2], next: "last", speed: .6 },
        last: { frames: [3, 4, 5], next: "first", speed: .6 }
    }
};

ROE.vovAnimations.tavern_glow_5_n = {
    pause: 5000,
    spritesheet: "https://static.realmofempires.com/images/anim/n_tavern3_glow.png",
    width: 145,
    height: 157,
    animations: {
        first: { frames: [0, 1, 2], next: "last", speed: .6 },
        last: { frames: [3, 4, 5], next: "first", speed: .6 }
    }
};

ROE.vovAnimations.torches_n = {
    spritesheet: "https://static.realmofempires.com/images/anim/torches.png",
    width: 222,
    height: 167,
    animations: {
        first: { frames: [0, 1, 2], next: "last", speed: 1 },
        last: { frames: [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.ridge_civilian_red_d = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/ridgeCivRedD.png',
    width: 21,
    height: 36,
    animations: {
        first: { frames: [0, 0], next: "last", speed: .05 },
        last: { frames: [1, 2, 3, 2, 3, 2, 3, 2, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.ridge_civilian_red_s = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/ridgeCivRedS.png',
    width: 29,
    height: 29,
    animations: {
        first: { frames: [0, 0], next: "talk", speed: .07 },
        talk: { frames: [1, 0, 1, 0, 1, 0, 1, 0, 1], next: "wait", speed: 1 },
        wait: { frames: [0, 0], next: "talk2", speed: .1 },
        talk2: { frames: [1, 0, 2, 3, 4, 5, 6], next: "wait2", speed: 1 },
        wait2: { frames: [5, 5], next: "last", speed: .15 },
        last: { frames: [6, 5, 7, 8, 7, 5, 6, 5, 7, 8, 7, 5, 6, 8, 5, 9, 3, 10], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.sitting_at_well = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/sittingAtWell.png',
    width: 31,
    height: 30,
    animations: {
        first: { frames: [0, 0], next: "raise", speed: 12 },
        raise: { frames: [1, 2, 3, 4, 5], next: "point", speed: 1 },
        point: { frames: [6, 7, 6, 7, 6, 7, 7, 7, 7, 7, 6], next: "lower", speed: 1 },
        lower: { frames: [5, 4, 3, 2, 1], next: "wait", speed: 1 },
        wait: { frames: [0, 0], next: "raise2", speed: .07 },
        raise2: { frames: [1, 2, 3, 4, 5], next: "point2", speed: 1 },
        point2: { frames: [6, 7, 6], next: "scratch", speed: 1 },
        scratch: { frames: [8, 9, 10, 11, 10, 11, 10, 11, 10, 11, 10, 11, 10, 11, 10, 11, 10, 9, 8], next: "last", speed: 1 },
        last: { frames: [5, 4, 3, 2, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.lute_player = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/lutePlayer.png',
    width:31,
    height:53,
    animations: {
        first: { frames: [0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3], next: "last", speed: 1 },
        last: { frames: [0, 4, 5, 6, 5, 4, 5, 6, 5, 4, 5, 6, 0, 7, 8, 9, 10, 7, 8, 9], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.preacher = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/preacher.png',
    width:39,
    height:42,
    animations: {
        first:{ frames:[0,1,2,3,4], next:"mid", speed: 1},
        mid: { frames: [5, 5], next: "last", speed: .1 },
        last:{ frames:[6,7,8,9,10,11,11,11,11,12,13,14,15,15,15,15,16,17,16,15,15,15,16,17,16,15,15,16,17,16,15,16,17,16,15,16,17,16,15,15,15,15,18,19,20,21,21], next:"first", speed: 1}
    }
};

ROE.vovAnimations.follower_brown_s = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/followerBrownS.png',
    width:28,
    height:33,
    animations: {
        first:{ frames:[0,0], next:"raise", speed: .05},
        raise:{ frames:[1,2,3,3,3,4,3,4], next:"wait", speed: 1},
        wait: { frames: [3, 3], next: "wave", speed: .1 },
        wave:{ frames:[4,3,4,3,4], next:"last", speed: 1},
        last:{ frames:[3,2,1], next:"first", speed: 1}
    }
};

ROE.vovAnimations.follower_grey_d = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/followerGreyD.png',
    width:29,
    height:35,
    animations: {
        first:{ frames:[0,0], next:"raise", speed: .05},
        raise:{ frames:[1,2,3,4,5,6], next:"wait", speed: 1},
        wait: { frames: [7, 7], next: "last", speed: .1 },
        last:{ frames:[6,5,4,3,2,1], next:"first", speed: 1}
    }
};

ROE.vovAnimations.follower_grey_s = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/followerGreyS.png',
    width:25,
    height:43,
    animations: {
        first:{ frames:[0,0], next:"raise", speed: .1},
        raise:{ frames:[1,2], next:"wait", speed: 1},
        wait:{ frames:[3,3], next:"last", speed: .03},
        last:{ frames:[2,1], next:"first", speed: 1}
    }
};

ROE.vovAnimations.follower_purple_s = {
    pause: 5000,
    spritesheet: 'https://static.realmofempires.com/images/anim/followerPurpleS.png',
    width: 24,
    height: 41,
    animations: {
        first: { frames: [0, 0], next: "raise", speed: .07 },
        raise: { frames: [1, 2, 3], next: "wait", speed: 1 },
        wait: { frames: [4, 4, 4], next: "last", speed: .07 },
        last: { frames: [3, 2, 1], next: "first", speed: 1 }
    }
};

ROE.vovAnimations.crone = {
    spritesheet: 'https://static.realmofempires.com/images/anim/cronereveal.png',
    width: 38,
    height: 44,
    animations: {
        first: { frames: [1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 0], next: "reveal", speed: 1 },
        reveal: { frames: [6, 7, 8, 9, 10, 11, 12, 13], next: "walk", speed: .5 },
        walk: { frames: [14, 15, 16, 17, 18, 19, 14, 15, 16, 17], next: "conceal", speed: 1 },
        conceal: { frames: [13, 12, 11, 10, 9, 8, 7, 6], next: "first", speed: .5 }
    }
};