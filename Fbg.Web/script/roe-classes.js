
ROE.Class = {};

ROE.Class.Village = function (id, name, x, y) {
    this.id = id;
    this.name = name;
    this.x = x;
    this.y = y;
}
ROE.Class.Village.prototype =
{
    id: undefined
    , name: undefined
    , x: undefined
    , y: undefined
    , getFormatedName: function getFormatedName() {
        return "%name%(%x%, %y%)".format(this);
    }

}



ROE.Class.Player = function (id, name) {
    this.id = id;
    this.name = name;
}
ROE.Class.Player.prototype =
{
    id: undefined
    , name: undefined

}
