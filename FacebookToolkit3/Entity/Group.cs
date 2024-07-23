using System;
using System.Drawing;
using System.Net;
using System.Xml.Serialization;
using Facebook.Utility;

namespace Facebook.Entity {
    public enum GroupType
    {
        Unknown,
        College,
        HighSchool,
        Work,
        Region
    }

    [Serializable]
    public class Group
    {
        #region Private Data

        private string _groupId;
        private string _networkId;
        private string _name;
        private string _type;
        private string _subType;
        private string _recentNews;
        private string _creator;
        private DateTime _updateDate;
        private string _office;
        private string _webSite;
        private Location _venue;
        private Image _picture;
        private byte[] _pictureBytes;
        private Uri _pictureUrl;
        private Image _pictureBig;
        private byte[] _pictureBigBytes;
        private Uri _pictureBigUrl;
        private Image _pictureSmall;
        private byte[] _pictureSmallBytes;
        private Uri _pictureSmallUrl;
        private string _description;


        #endregion Private Data

        #region Properties
        /// <summary>
        /// The facebook unique identifier of the group
        /// </summary>
        public string GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }
        /// <summary>
        /// The facebook unique identifier of the network that the group is affiliated with
        /// </summary>
        public string NetworkId
        {
            get { return _networkId; }
            set { _networkId = value; }
        }
        /// <summary>
        /// The name of the group
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// The type of group
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// The sub-type of group
        /// </summary>
        public string SubType
        {
            get { return _subType; }
            set { _subType = value; }
        }
        /// <summary>
        /// Any news about the group
        /// </summary>
        public string RecentNews
        {
            get { return _recentNews; }
            set { _recentNews = value; }
        }
        /// <summary>
        /// The name of the group creator
        /// </summary>
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }
        /// <summary>
        /// The last time the group was updated
        /// </summary>
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }
        /// <summary>
        /// The description of the group office
        /// </summary>
        public string Office
        {
            get { return _office; }
            set { _office = value; }
        }
        /// <summary>
        /// Link to group's website 
        /// </summary>
        public string WebSite
        {
            get { return _webSite; }
            set { _webSite = value; }
        }
        /// <summary>
        /// Location of group's headquarters
        /// </summary>
        public Location Venue
        {
            get { return _venue; }
            set { _venue = value; }
        }
        /// <summary>
        /// The picture of the group.  This is not initially populated, but when accessed will stream the bytes of the picture
        /// from the url and provide an actual picture
        /// </summary>
        [XmlIgnore()]
        public Image Picture
        {
            get
            {
                if (_pictureUrl == null)
                {
                    return Facebook.Properties.Resources.missingPicture;
                }
                else if (_picture == null)
                {
                    // just used to access the property and populate the fields
                    byte[] bytes = PictureBytes;
                }
                    
                return _picture;
            }
            set { _picture = value; }
        }
        /// <summary>
        /// This is only used for serialization.  Should not be accessed directly.
        /// </summary>
        [XmlElementAttribute("Picture")]
        public byte[] PictureBytes
        {
            get
            {
                if (_pictureUrl == null)
                {
                    return null;
                }
                else if (_pictureBytes == null)
                {
                    WebClient webClient = new WebClient();
                    _pictureBytes = webClient.DownloadData(_pictureUrl);
                    _picture = ImageHelper.ConvertBytesToImage(_pictureBytes);
                }

                return _pictureBytes;
            }
        }
        /// <summary>
        /// The url of the group picture
        /// </summary>
        public Uri PictureUrl
        {
            get
            {
                if (_pictureUrl == null)
                {
                    return new Uri(Facebook.Properties.Resources.MissingPictureUrl);
                }
                else
                {
                    return _pictureUrl;
                }
            }
            set { _pictureUrl = value; }
        }
        /// <summary>
        /// The picture of the group (big version).  This is not initially populated, but when accessed will stream the bytes of the picture
        /// from the url and provide an actual picture
        /// </summary>
        [XmlIgnore()]
        public Image PictureBig
        {
            get
            {
                if (_pictureBigUrl == null)
                {
                    return Facebook.Properties.Resources.missingPicture;
                }
                else if (_pictureBig == null)
                {
                    // just used to access the property and populate the fields
                    byte[] bytes = PictureBigBytes;
                }

                return _pictureBig;
            }
            set { _pictureBig = value; }
        }
        /// <summary>
        /// This is only used for serialization.  Should not be accessed directly.
        /// </summary>
        [XmlElementAttribute("Picture")]
        public byte[] PictureBigBytes
        {
            get
            {
                if (_pictureBigUrl == null)
                {
                    return null;
                }
                else if (_pictureBigBytes == null)
                {
                    WebClient webClient = new WebClient();
                    _pictureBigBytes = webClient.DownloadData(_pictureBigUrl);
                    _pictureBig = ImageHelper.ConvertBytesToImage(_pictureBigBytes);
                }

                return _pictureBigBytes;
            }
        }
        /// <summary>
        /// The url of the group picture (big version)
        /// </summary>
        public Uri PictureBigUrl
        {
            get
            {
                if (_pictureBigUrl == null)
                {
                    return new Uri(Facebook.Properties.Resources.MissingPictureUrl);
                }
                else
                {
                    return _pictureBigUrl;
                }
            }
            set
            {
                _pictureBigUrl = value;
            }
        }
        /// <summary>
        /// The picture of the group (small version).  This is not initially populated, but when accessed will stream the bytes of the picture
        /// from the url and provide an actual picture
        /// </summary>
        [XmlIgnore()]
        public Image PictureSmall
        {
            get
            {
                if (_pictureSmallUrl == null)
                {
                    return Facebook.Properties.Resources.missingPicture;
                }
                else if (_pictureSmall == null)
                {
                    // just used to access the property and populate the fields
                    byte[] bytes = PictureSmallBytes;
                }

                return _pictureSmall;
            }
            set { _pictureSmall = value; }
        }
        /// <summary>
        /// This is only used for serialization.  Should not be accessed directly.
        /// </summary>
        [XmlElementAttribute("Picture")]
        public byte[] PictureSmallBytes
        {
            get
            {
                if (_pictureSmallUrl == null)
                {
                    return null;
                }
                else if (_pictureSmallBytes == null)
                {
                    WebClient webClient = new WebClient();
                    _pictureSmallBytes = webClient.DownloadData(_pictureSmallUrl);
                    _pictureSmall = ImageHelper.ConvertBytesToImage(_pictureSmallBytes);
                }

                return _pictureSmallBytes;
            }
        }
        /// <summary>
        /// The url of the group picture (small version)
        /// </summary>
        public Uri PictureSmallUrl
        {
            get
            {
                if (_pictureSmallUrl == null)
                {
                    return new Uri(Facebook.Properties.Resources.MissingPictureUrl);
                }
                else
                {
                    return _pictureSmallUrl;
                }
            }
            set
            {
                _pictureSmallUrl = value;
            }
        }
        /// <summary>
        /// The description of the group
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion Properties

        /// <summary>
        /// default constructor
        /// </summary>
        public Group()
        {
        }
    }
}