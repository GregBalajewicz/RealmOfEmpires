using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Fbg.Bll;
using System.Collections.Generic;
using Fbg.Common;

/// <summary>
/// Summary description for VillageOverviewImages
/// </summary>
public class VillageOverviewImages
{
    public class Levels
    {
        //public static []_lvls = new int[];
        private static readonly Dictionary<int,int[]> _lvls;

         static Levels()
        {
            _lvls = new Dictionary<int, int[]>(13);
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.VillageHQ, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Barracks, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.CoinMine, new int[]{999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.HidingSpot, new int[]{999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Palace, new int[]{999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Stable, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.TradePost, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Treasury, new int[]{10, 20, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Tavern, new int[]{2, 4, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Farmland, new int[]{5, 10, 17, 22, 25, 29, 999});
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.Wall, new int[] { 2, 3, 5, 7, 8, 9, 999 });
            _lvls.Add(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower, new int[] { 2, 5, 6, 7, 8, 999 });
        }

        public static int[] LevelsForBuilding(int buildingID) 
        {
            return _lvls[buildingID];
        }
    }



    // /// <summary>
    ///// 
    ///// </summary>
    ///// <param name="timeofday">pass null if you want neutral</param>
    ///// <param name="r"></param>
    ///// <returns></returns>
    //public static string ImageDirPrefix(TimeOfDay timeofday)
    //{
    //    return ImageDirPrefix(timeofday, null);
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static string ImageDirPrefix_FORD1(TimeOfDay timeofday)
    {
        if (timeofday == TimeOfDay.day)
        {

            return "https://static.realmofempires.com/images/vov/d_";

        }
        else
        {

            return "https://static.realmofempires.com/images/vov/n_";

        }

    }
    public static string ImageDirPrefix(TimeOfDay timeofday, Realm r)
    {
        if (timeofday == TimeOfDay.day)
        {
            if (r == null || r.Theme == Realm.Themes.desert)
            {
                return "https://static.realmofempires.com/images/vov/desert/d_";
            }
            else
            {
                return "https://static.realmofempires.com/images/vov/d_";
            }
        }
        else
        {
            if (r == null || r.Theme == Realm.Themes.desert)
            {
                return "https://static.realmofempires.com/images/vov/desert/n_";
            }
            else
            {
                return "https://static.realmofempires.com/images/vov/n_";
            }
        }
    }

   
    //private static readonly string imageDirPrefix_day = "https://static.realmofempires.com/images/vov/d_";
    //public static readonly string imageDirPrefix_night = "https://static.realmofempires.com/images/vov/n_";
   // public static readonly string imageDirPrefix_neutral = "https://static.realmofempires.com/images/vov/";
    private static readonly string imageMiscDirPrefix = "https://static.realmofempires.com/images/misc/";
    private  static readonly string imageMisc2DirPrefix = "https://static.realmofempires.com/images/BuildingImages/";

    int _towersLevel;
    int _wallLevel;

    public VillageOverviewImages(int towersLevel, int wallLevel)
    {
        _towersLevel = towersLevel;
        _wallLevel = wallLevel;
    }

    /// <summary>
    /// Returns null if no image should be shown
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    public string[] GetTowersImageUrl(int level, TimeOfDay timeofDay, out string Left_htmlMapAreCoords, out string Right_htmlMapAreCoords, Realm r)
    {
        string imageDirPrefix = ImageDirPrefix(timeofDay, r);
        Left_htmlMapAreCoords = String.Empty;
        Right_htmlMapAreCoords = String.Empty;
        int wallLevel = _wallLevel;

        if (level == 0)
        {
            Left_htmlMapAreCoords = "47,494,48,423,37,381,47,383,64,415,73,422,76,505";
            Right_htmlMapAreCoords = "395,499,396,419,424,416,426,493";
            return new string[3] { imageDirPrefix + "tower1L.png", "", imageDirPrefix + "Tower1R.png" };
        }
        else if (level <= 2)
        {
            Left_htmlMapAreCoords = "47,494,48,423,37,381,47,383,64,415,73,422,76,505";
            Right_htmlMapAreCoords = "395,499,396,419,424,416,426,493";
            return new string[3] { imageDirPrefix + "tower1L.png", "", imageDirPrefix + "Tower1R.png" };
        }
        else if (level <= 5)
        {
            Left_htmlMapAreCoords = "50,490,39,385,63,369,83,393,77,498";
            Right_htmlMapAreCoords = "398,502,391,392,409,365,427,398,423,499";           
            return new string[3] { imageDirPrefix + "tower3L.png", "", imageDirPrefix + "Tower3R.png" };
        }
        else if (level <= 6)
        {
            Left_htmlMapAreCoords = "50,490,39,385,63,369,83,393,77,498";
            Right_htmlMapAreCoords = "398,502,391,392,409,365,427,398,423,499";
            return new string[3] { imageDirPrefix + "tower7L.png", "", imageDirPrefix + "Tower7R.png" };
        }
        else if (level <= 7)
        {
            Left_htmlMapAreCoords = "36,476,43,358,66,348,85,357,84,443,65,455,67,493";
            Right_htmlMapAreCoords = "383,451,388,358,409,349,431,358,431,498,403,501,406,459";
            return new string[3] { imageDirPrefix + "tower8L.png", "", imageDirPrefix + "Tower8R.png" };
        }
        else if (level <= 8)
        {
            Left_htmlMapAreCoords = "22,479,33,372,30,349,62,341,88,353,90,432,69,454,66,499";
            Right_htmlMapAreCoords = "382,440,384,356,416,347,444,358,446,492,412,505,405,504,404,451";
            return new string[3] { imageDirPrefix + "tower9L.png", "", imageDirPrefix + "Tower9R.png" };
        }
        else
        {
            Left_htmlMapAreCoords = "42,480,38,355,55,345,91,351,91,419,71,432,71,494";
            Right_htmlMapAreCoords = "380,423,378,353,407,347,434,355,431,492,404,495,401,427";
            return new string[3] { imageDirPrefix + "tower10L.png", "", imageDirPrefix + "Tower10R.png" };
        }
    }
    /// <summary>
    /// Returns null if no image should be shown
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    public string[] GetWallImageUrl(int level, TimeOfDay timeofDay, out string htmlMapAreCoords, Realm r)
    {
        string imageDirPrefix;
        if (r == null)
        {
            imageDirPrefix = ImageDirPrefix_FORD1(timeofDay);
        }
        else
        {
            imageDirPrefix = ImageDirPrefix(timeofDay, r);
        }
        int towersLevel = _towersLevel;
        htmlMapAreCoords = String.Empty;
        if (level == 0)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "78,475,206,496,202,465,275,465,276,493,387,472,388,497,273,523,212,528,74,512";
                return new string[3] { imageDirPrefix + "Wall1L.png", imageDirPrefix + "Wall1f.png", imageDirPrefix + "Wall1R.png" };
            }
            else if (towersLevel <= 5)
            {

                return new string[3] { imageDirPrefix + "Wall_XXL_WT.png", imageDirPrefix + "Wall_XXF_WT.png", imageDirPrefix + "Wall_XXR_WT.png" };
            }
            else
            {

                return new string[3] { imageDirPrefix + "Wall_XXL_ST.png", imageDirPrefix + "Wall_XXF_ST.png", imageDirPrefix + "Wall_XXR_ST.png" };
            }
        }
        if (level <= 2)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "78,475,206,496,202,465,275,465,276,493,387,472,388,497,273,523,212,528,74,512";
                return new string[3] { imageDirPrefix + "Wall1L.png", imageDirPrefix + "Wall1f.png", imageDirPrefix + "Wall1R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "78,475,206,496,202,465,275,465,276,493,387,472,388,497,273,523,212,528,74,512";
                return new string[3] { imageDirPrefix + "Wall1L.png", imageDirPrefix + "Wall1F_WT.png", imageDirPrefix + "Wall1R.png" };
            }
            else
            {
                htmlMapAreCoords = "78,475,206,496,202,465,275,465,276,493,387,472,388,497,273,523,212,528,74,512";
                return new string[3] { imageDirPrefix + "Wall1L.png", imageDirPrefix + "Wall1F_ST.png", imageDirPrefix + "Wall1R.png" };
            }
        }
        if (level <= 3)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall3L.png", imageDirPrefix + "Wall3f.png", imageDirPrefix + "Wall3R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall3L.png", imageDirPrefix + "Wall3F_WT.png", imageDirPrefix + "Wall3R.png" };
            }
            else
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall3L.png", imageDirPrefix + "Wall3F_ST.png", imageDirPrefix + "Wall3R.png" };
            }
        }
        else if (level <= 5)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall5L.png", imageDirPrefix + "Wall5f.png", imageDirPrefix + "Wall5R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall5L.png", imageDirPrefix + "Wall5F_WT.png", imageDirPrefix + "Wall5R.png" };
            }
            else
            {
                htmlMapAreCoords = "80,460,202,481,204,464,275,463,277,476,338,471,395,457,394,498,350,515,278,516,267,526,213,525,203,517,130,514,79,505";
                return new string[3] { imageDirPrefix + "Wall5L.png", imageDirPrefix + "Wall5F_ST.png", imageDirPrefix + "Wall5R.png" };
            }
        }
        else if (level <= 7)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "70,500,169,518,171,530,291,531,292,520,392,501,393,465,379,452,285,469,282,450,192,446,180,463,134,461,83,446,67,459";
                return new string[3] { imageDirPrefix + "Wall7L.png", imageDirPrefix + "Wall7f.png", imageDirPrefix + "Wall7R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "70,500,169,518,171,530,291,531,292,520,392,501,393,465,379,452,285,469,282,450,192,446,180,463,134,461,83,446,67,459";
                return new string[3] { imageDirPrefix + "Wall7L.png", imageDirPrefix + "Wall7F_WT.png", imageDirPrefix + "Wall7R.png" };
            }
            else
            {
                htmlMapAreCoords = "70,500,169,518,171,530,291,531,292,520,392,501,393,465,379,452,285,469,282,450,192,446,180,463,134,461,83,446,67,459";
                return new string[3] { imageDirPrefix + "Wall7L.png", imageDirPrefix + "Wall7F_ST.png", imageDirPrefix + "Wall7R.png" };
            }
        }
        else if (level <= 8)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "69,497,168,519,168,531,290,531,291,519,401,498,403,454,380,442,282,458,280,449,192,443,184,458,93,437,72,452";
                return new string[3] { imageDirPrefix + "Wall8L.png", imageDirPrefix + "Wall8f.png", imageDirPrefix + "Wall8R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "69,497,168,519,168,531,290,531,291,519,401,498,403,454,380,442,282,458,280,449,192,443,184,458,93,437,72,452";
                return new string[3] { imageDirPrefix + "Wall8L.png", imageDirPrefix + "Wall8F_WT.png", imageDirPrefix + "Wall8R.png" };
            }
            else
            {
                htmlMapAreCoords = "69,497,168,519,168,531,290,531,291,519,401,498,403,454,380,442,282,458,280,449,192,443,184,458,93,437,72,452";
                return new string[3] { imageDirPrefix + "Wall8L.png", imageDirPrefix + "Wall8F_ST.png", imageDirPrefix + "Wall8R.png" };
            }
        }
        else if (level <= 9)
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall9L.png", imageDirPrefix + "Wall9f.png", imageDirPrefix + "Wall9R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall9L.png", imageDirPrefix + "Wall9F_WT.png", imageDirPrefix + "Wall9R.png" };
            }
            else
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall9L.png", imageDirPrefix + "Wall9F_ST.png", imageDirPrefix + "Wall9R.png" };
            }
        }
        else
        {
            if (towersLevel == 0)
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall10L.png", imageDirPrefix + "Wall10f.png", imageDirPrefix + "Wall10R.png" };
            }
            else if (towersLevel <= 5)
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall10L.png", imageDirPrefix + "Wall10F_WT.png", imageDirPrefix + "Wall10R.png" };
            }
            else
            {
                htmlMapAreCoords = "73,495,169,515,174,529,293,530,294,515,398,492,398,433,381,422,286,436,282,419,193,421,181,434,94,421,71,432";
                return new string[3] { imageDirPrefix + "Wall10L.png", imageDirPrefix + "Wall10F_ST.png", imageDirPrefix + "Wall10R.png" };
            }
        }
    }
    /// <summary>
    /// Returns null if no image should be shown
    /// </summary>
    /// <param name="bt"></param>
    /// <param name="htmlMapAreCoords">returns String.Empty if building is not built hence no clickable area should be available</param>
    /// <returns></returns>
    public string GetBuildingImageUrl(Fbg.Bll.BuildingType bt, int level, TimeOfDay timeOfDay, out string htmlMapAreCoords, VillageImgPack imgPack, ref int buildingStage, bool doAnimation, Realm r)
    {
        string imageDirPrefix;
        if (r == null)
        {
            imageDirPrefix = ImageDirPrefix_FORD1(timeOfDay);
        }
        else
        {
            imageDirPrefix = ImageDirPrefix(timeOfDay, r);
        }
        htmlMapAreCoords = String.Empty;
        string animPrexix = doAnimation ? "anim_" : "";

        switch (bt.ID)
        {
            case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                if (level <= 10)
                {
                    htmlMapAreCoords = "169,199,169,148,223,148,223,138,237,121,257,140,257,203,184,207";
                    buildingStage = 1;
                    return imageDirPrefix + animPrexix + "HQ1.png";
                }
                else if (level <= 20)
                {
                    htmlMapAreCoords = "171,202,171,149,225,150,225,126,241,106,259,127,257,206,186,206";
                    buildingStage = 2;
                    return imageDirPrefix + animPrexix + "HQ2.png";
                }
                else if (level > 20)
                {
                    htmlMapAreCoords = "165,100,150,117,155,205,188,213,251,209,252,150,240,142,218,143,223,122,208,104,185,103";
                    buildingStage = 3;
                    return imageDirPrefix + animPrexix + "HQ3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                if (level <= 10)
                {
                    htmlMapAreCoords = "74,347,75,303,125,282,147,287,148,332,99,357";
                    buildingStage = 1;
                    return imageDirPrefix + animPrexix + "barracks1.png";
                }
                else if (level <= 20)
                {
                    buildingStage = 2;
                    htmlMapAreCoords ="121,367,143,358,166,361,200,340,170,329,169,296,121,283,75,303,73,347,122,364";
                    return imageDirPrefix + animPrexix + "barracks2.png";
                }
                else if (level > 20)
                {
                    buildingStage = 3;
                    htmlMapAreCoords ="53,348,81,360,118,361,161,352,184,342,185,301,123,281,100,289,77,280,57,287";
                    return imageDirPrefix + animPrexix + "barracks3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                htmlMapAreCoords = "20,61,150,129";
                if (level <= 4)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine1.png", imageDirPrefix + "SilverMine1.png");
                }
                else if (level <= 9)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine2.png", imageDirPrefix + "SilverMine2.png");
                }
                else if (level <= 13)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine3.png", imageDirPrefix + "SilverMine3.png");
                }
                else if (level <= 18)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine4.png", imageDirPrefix + "SilverMine4.png");
                }
                else if (level <=22)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine5.png", imageDirPrefix + "SilverMine5.png");
                }
                else if (level <= 27)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine6.png", imageDirPrefix + "SilverMine6.png");
                }
                else if (level <= 31)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine7.png", imageDirPrefix + "SilverMine7.png");
                }
                else if (level <= 36)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine8.png", imageDirPrefix + "SilverMine8.png");
                }
                else if (level <= 44)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine9.png", imageDirPrefix + "SilverMine9.png");
                }
                else if (level > 44)
                {
                    return imgPack.Image(imageDirPrefix + "SilverMine10.png", imageDirPrefix + "SilverMine10.png");
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                    htmlMapAreCoords = "431,332,463,332,463,391,431,390,431,332";
                    return imageDirPrefix + "HidingPlace.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                    htmlMapAreCoords = "177,107,246,90,340,103,325,74,255,43,229,4,204,6,207,61,154,65";
                    return imageDirPrefix + "palace.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                if (level <= 10)
                {
                    htmlMapAreCoords ="399,101,441,103,440,56,397,56";
                    return imageDirPrefix + "siege1.png";
                }
                else if (level <= 20)
                { 
                    htmlMapAreCoords ="401,96,453,104,435,57,399,57";
                    return imageDirPrefix + "siege2.png";
                }
                else if (level > 20)
                {
                    htmlMapAreCoords ="388,95,454,107,434,56,393,56";
                    return imageDirPrefix + "siege3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                if (level == 0)
                {
                    htmlMapAreCoords = "46,240,101,251,128,219,119,191,68,184,48,212";
                    return imageDirPrefix + animPrexix + "stable1.png";
                }
                else if (level <= 10)
                {
                    buildingStage = 1;
                    htmlMapAreCoords = "46,240,101,251,128,219,119,191,68,184,48,212";
                    return imageDirPrefix + animPrexix + "stable1.png";
                }
                else if (level <= 20)
                {
                    buildingStage = 2;
                    htmlMapAreCoords = "69,257,131,225,123,183,64,185,30,219,30,255";
                    return imageDirPrefix + animPrexix + "stable2.png";
                }
                else if (level > 20)
                {
                    buildingStage = 3;
                    htmlMapAreCoords = "69,257,131,225,123,183,64,185,30,219,30,255";
                    return imageDirPrefix + animPrexix + "stable3.png";
                }

                break;
            case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                if (level <= 10)
                {
                    htmlMapAreCoords = "379,293,380,237,445,229,447,279,469,284,469,301,423,323";
                    return imageDirPrefix + "trade1.png";
                }
                else if (level <= 20)
                {
                    htmlMapAreCoords ="377,297,423,314,467,298,460,227,409,225";
                    return imageDirPrefix + "trade2.png";
                }
                else if (level > 20)
                { 
                    htmlMapAreCoords ="371,298,422,320,471,302,471,285,460,282,459,251,373,240";
                    return imageDirPrefix + "trade3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                if (level <= 10)
                {
                    buildingStage = 1;
                    htmlMapAreCoords = "284,217,308,224,337,217,333,180,287,149";
                    return imageDirPrefix + animPrexix + "treasury1.png";
                }
                else if (level <= 20)
                {
                    buildingStage = 2;
                    htmlMapAreCoords = "285,213,336,237,365,215,362,173,288,150";
                    return imageDirPrefix + animPrexix + "treasury2.png";
                }
                else if (level > 20)
                {
                    buildingStage = 3;
                    htmlMapAreCoords = "283,222,335,235,356,221,360,159,309,149,280,155";
                    return imageDirPrefix + animPrexix + "treasury3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                if (level <= 2)
                {
                    htmlMapAreCoords = "258,388,307,416,358,387,356,338,303,283,256,296";
                    buildingStage = 1;
                    return imageDirPrefix + animPrexix + "tavern1.png";
                }
                else if (level <= 4)
                {
                    htmlMapAreCoords ="309,412,358,391,357,334,306,283,255,297,262,394";
                    buildingStage = 1;
                    return imageDirPrefix + animPrexix + "tavern2.png";
                }
                else if (level >= 5)
                { 
                    htmlMapAreCoords ="258,388,307,416,358,387,356,338,303,283,256,296";
                    buildingStage = 1;
                    return imageDirPrefix + animPrexix + "tavern3.png";
                }
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                if (level <= 5)
                {
                    htmlMapAreCoords = "354,142,395,162,462,137,405,100,357,113";
                    return imageDirPrefix + "farm1.png";
                }
                else if (level <= 10)
                {
                    htmlMapAreCoords = "354,143,394,161,469,140,442,114,356,105";
                    return imageDirPrefix + "farm2.png";
                }
                else if (level <= 17)
                {
                    htmlMapAreCoords = "354,139,422,195,476,205,472,145,401,103,354,113";
                    return imageDirPrefix + "farm3.png";
                }
                else if (level <= 22)
                { 
                    htmlMapAreCoords ="353,142,423,198,476,205,475,123,351,108";
                    return imageDirPrefix + "farm4.png";
                }
                else if (level <= 25)
                {
                    htmlMapAreCoords = "353,142,423,198,476,205,475,123,351,108";
                    return imageDirPrefix + "farm5.png";
                }
                else if (level <= 29)
                {
                    htmlMapAreCoords ="340,158,393,198,473,201,475,126,453,117,360,106";
                    return imageDirPrefix + "farm6.png";
                }
                else if (level <= 30)
                {
                    htmlMapAreCoords ="340,158,393,198,473,201,475,126,453,117,360,106";
                    return imageDirPrefix + "farm7.png";
                }
                break;
            default:
                throw new Exception("Unrecognized bt.ID=" + bt.ID.ToString());

        }
        return null;

    }

    public static string GetBuildingIconUrl(Fbg.Bll.BuildingType bt)
    {
        switch (bt.ID)
        {
            case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                return "https://static.realmofempires.com/images/BuildingIcons/hq.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                return "https://static.realmofempires.com/images/BuildingIcons/barracks.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                return "https://static.realmofempires.com/images/BuildingIcons/mine.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                return "https://static.realmofempires.com/images/BuildingIcons/palace.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                return "https://static.realmofempires.com/images/BuildingIcons/seige.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                return "https://static.realmofempires.com/images/BuildingIcons/stable.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                return "https://static.realmofempires.com/images/BuildingIcons/trade.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                return "https://static.realmofempires.com/images/BuildingIcons/treasury.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                return "https://static.realmofempires.com/images/BuildingIcons/wall.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                return "https://static.realmofempires.com/images/BuildingIcons/tavern.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                return "https://static.realmofempires.com/images/BuildingIcons/farm2.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                return "https://static.realmofempires.com/images/BuildingIcons/tower.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                return "https://static.realmofempires.com/images/BuildingIcons/HidingSpot.png";
                break;
            default:
                throw new Exception("Unrecognized bt.ID=" + bt.ID.ToString());

        }
        return null;

    }

    /// <summary>
    /// DEPRECIATED. D1 only 
    /// </summary>
    /// <param name="buildingID"></param>
    /// <param name="isAvailable"></param>
    /// <returns></returns>
    public static string GetBuildingImageForRecruitmentScreen(int buildingID, bool isAvailable)
    {
        switch (buildingID)
        {
            case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                return imageMiscDirPrefix + (isAvailable ? "barracks.png" : "barracks_gray.png");
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                return imageMiscDirPrefix + (isAvailable ? "palace.png" : "palace_gray.png");
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                return imageMiscDirPrefix + (isAvailable ? "siege.png" : "siege_gray.png");
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                return imageMiscDirPrefix + (isAvailable ? "stable.png" : "stable_gray.png");
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                return imageMiscDirPrefix + (isAvailable ? "tavern.png" : "tavern_gray.png");
                break;
            default:
                throw new Exception("Unrecognized buildingID=" + buildingID.ToString());

        }
        return null;

    }

    /// <summary>
    /// DEPRECIATED. D1 only
    /// </summary>
    /// <param name="buildingID"></param>
    /// <returns></returns>
    public static string GetBuildingImageForHelpScreen(int buildingID)
    {
        switch (buildingID)
        {
            case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                return imageMiscDirPrefix + "HQ.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                return imageMiscDirPrefix + "barracks.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                return imageMiscDirPrefix + "mine.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                return imageMiscDirPrefix + "palace.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                return imageMiscDirPrefix + "siege.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                return imageMiscDirPrefix + "stable.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                return imageMiscDirPrefix + "trade.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                return imageMiscDirPrefix + "treasury.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                return imageMiscDirPrefix + "wall.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                return imageMiscDirPrefix + "tavern.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                return imageMiscDirPrefix + "farm.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                return imageMiscDirPrefix + "Tower_S3R.png";
                break;
            case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                return imageMiscDirPrefix + "HidingSpot.png";
                break;
            default:
                throw new Exception("Unrecognized buildingID=" + buildingID.ToString());

        }
        return null;

    }

    private static int GetBuildingImageNumberBasedOnLevel(int[] levels, int level)
    {
        for (int i = 0; i < levels.Length; i++ )
        {
            if (level <= levels[i])
            {
                return i;
            }
        }
        return 0;
    }

    public static string GetBuildingImageForBuildingScreen(int buildingID, int level)
    {
        int[] lvls = Levels.LevelsForBuilding(buildingID);
        switch (buildingID)
        {
            case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                return String.Format("{0}bg_HQ{1}_brown.jpg", imageMisc2DirPrefix,  GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                return String.Format("{0}bg_barracks{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                return String.Format("{0}bg_mine{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                return String.Format("{0}bg_hidingspot{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                return String.Format("{0}bg_palace{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                return String.Format("{0}bg_siege{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                return String.Format("{0}bg_stables{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                return String.Format("{0}bg_trade{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                return String.Format("{0}bg_treasury{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                return String.Format("{0}bg_tavern{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                return String.Format("{0}bg_farm{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                return String.Format("{0}bg_wall{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                return String.Format("{0}bg_towers{1}_brown.jpg", imageMisc2DirPrefix, GetBuildingImageNumberBasedOnLevel(lvls, level) + 1);
            default:
                throw new Exception("Unrecognized buildingID=" + buildingID.ToString());

        }
        return null;
    }

    public uint GetVillageBackgroundIndex(int villagePoints)
    {
        if (villagePoints < 70) return 1;
        else if (villagePoints < 400) return 2;
        else if (villagePoints < 800) return 3;
        else if (villagePoints < 1300) return 4;
        else if (villagePoints < 2000) return 5;
        else if (villagePoints < 5000) return 6;
        else if (villagePoints < 8000) return 7;
        else if (villagePoints < 10000) return 8;
        else if (villagePoints < 12500) return 9;
        else return 10;
    }


    public string GetVillageBackground(int villagePoints, TimeOfDay timeofDay, VillageImgPack imgPack, ref bool backgroundHasWater, Realm r)
    {
        string imgPrefix = ImageDirPrefix(timeofDay,r);

        return GetVillageBackground(villagePoints, timeofDay, imgPack, ref backgroundHasWater, imgPrefix);
    }

    public string GetVillageBackground_FORD1(int villagePoints, TimeOfDay timeofDay, VillageImgPack imgPack, ref bool backgroundHasWater)
    {
        string imgPrefix = ImageDirPrefix_FORD1(timeofDay);
        return GetVillageBackground(villagePoints, timeofDay, imgPack, ref backgroundHasWater, imgPrefix);

    }



    private string GetVillageBackground(int villagePoints, TimeOfDay timeofDay, VillageImgPack imgPack, ref bool backgroundHasWater, string imgPrefix)
    {
        uint imgIndex = GetVillageBackgroundIndex(villagePoints);

        if (imgIndex >= 7)
        {
            backgroundHasWater = true;
        }
        return imgPack.Image(imgPrefix + "bg" + imgIndex.ToString() + ".jpg", imgPrefix + "bg" + imgIndex.ToString() + ".jpg");
    }

}