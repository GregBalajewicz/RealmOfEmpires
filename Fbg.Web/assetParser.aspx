
<%@ Import Namespace="System.IO" %>



<script language="C#" runat="server">



    protected void Page_Load(object sender, EventArgs e)
    {

        ///Create a folder called AAA in Web root first, this tries to look at it for map assets
        string[] filePaths = Directory.GetFiles(Server.MapPath("~/AAA-SHIP/"));
        string mapFolder = "highlands"; //desert was des


        //STEP 1 - removes empty image files
        //processEmpty(filePaths);



        //STEP 2 - removes coll files, renames image files that have corresponding coll files
        //DANGER - ONLY RUN ONCE - AND ONLY WHEN RED COLL FILES EXIST then comment out!
        //processCollision(filePaths);



        //STEP 3
        //outputs SQL Inserts
        processInserts(filePaths, mapFolder);

    }


    bool emptyImage(System.Drawing.Bitmap image)
    {
        for (int y = 0; y < image.Height; ++y)
        {
            for (int x = 0; x < image.Width; ++x)
            {
                //first time we find a non invisble pixel, this image no longer empty
                //3 / 255  is about 1% visible
                if (image.GetPixel(x, y).A > 15)
                {
                    return false;
                }
            }
        }
        return true;
    }


    /// <summary>
    /// looks at all files, if they are empty (have no visible pixels) it deletes them
    /// </summary>
    void processEmpty(string[] filePaths)
    {

        //loop through files, find empty PNGs
        foreach (string filePath in filePaths)
        {

            //skip coll files, they get processed later
            if (filePath.Contains("_col"))
            {
                //continue;
            }

            //skip analysis on files bigger than certain size, we keep them
            long length = new System.IO.FileInfo(filePath).Length;
            if (length > 2000) {
                //assets.Text += "<div>" + filePath + " size(" + length + "): skipped.</div>";
                continue;
            }

            //skip analysis on files below certain size and delete
            if (length < 240) {
                //assets.Text += "<div>" + filePath + " size(" + length + "): skipped.</div>";
                File.Delete(filePath);
                continue;
            }

            //check files pixels to see if it has any visible data. Delete if invisible
            System.Drawing.Bitmap assetBMP = new System.Drawing.Bitmap(filePath);
            bool isAnEmptyImage = emptyImage(assetBMP);
            assetBMP.Dispose();

            if (isAnEmptyImage)
            {
                //assets.Text += "<div>" + filePath + " size(" + length + "): EMPTY</div>";
                File.Delete(filePath);
            }
            else
            {
                //assets.Text += "<div>" + filePath + " size(" + length + "): USED</div>";

            }

        }

    }





    /// <summary>
    /// Go through the folder, if a file has coll in its name, delete it, AND
    /// rename files that match teh coll name, based on the deleted version, to be coll
    /// </summary>
    void processCollision(string[] filePaths)
    {
        foreach (string filePath in filePaths)
        {

            string fileName = Path.GetFileName(filePath);

            string nonCollisionVersionPath;

            if (fileName.Contains("_col"))
            {
                //delete the asset that is there to indicate the ID of the real coll asset
                File.Delete(filePath);

                //from the name of the deleted asset we derive the name of the real coll asset
                nonCollisionVersionPath = filePath.Replace("_coll", "");
                if (File.Exists(nonCollisionVersionPath))
                {
                    //renames the real asset to indicate it is coll (doesnt allow vills)
                    File.Move(nonCollisionVersionPath, filePath);
                }
            }
        }
    }

    void processInserts(string[] filePaths, string mapFolder) {

        int landMarkID = 0;
        bool newLandmark = true;
        string previousLandMarkName = null;

        foreach (string filePath in filePaths)
        {

            string fileName = Path.GetFileName(filePath);
            string[] nameSplit = fileName.Remove(fileName.IndexOf('.')).Split('_'); //trim the xtension
            if (nameSplit.Length > 1)
            {


                //assets.Text += "<div>" + nameSplit[1] + "</div>";
                //assets.Text += "<div>" + fileName + "</div>";

                string name = nameSplit[1];

                newLandmark = !object.Equals(previousLandMarkName, name);
                previousLandMarkName = name;

                string chance = "10";
                string x = Int32.Parse(nameSplit[2].Substring(1)) + ""; //remove the X
                string y = nameSplit[3].Substring(1); //remove the Y
                int AllowVillage = 1; //allow placement of village on this part, by default

                //if name has col in it, it means dont place village on it
                if (fileName.Contains("_col"))
                {
                    AllowVillage = 0;
                }

                //since Y string value can have '-', we have to deal with
                if (y[0].Equals('-'))
                {
                    y = "-" + Int32.Parse(y.Substring(1));
                }
                else {
                    y = Int32.Parse(y) + "";
                }

                if (newLandmark)
                {
                    landMarkID++;
                    assets.Text += "<br/>";
                    assets.Text += "<div>insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition) values(" + landMarkID + ",'" + name + "'," + chance + ",0)</div>";
                }

                assets.Text += "<div>insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord,AllowVillage) "+
                    "values(" + landMarkID + ",'" + mapFolder + "/" + fileName + "'," + x + "," + y + "," + AllowVillage + ")</div>";



            }
        }
    }





</script>


<form runat=server>

    <asp:Label runat=server id=assets></asp:Label>

</form>