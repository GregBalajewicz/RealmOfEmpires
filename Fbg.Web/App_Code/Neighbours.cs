using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BDA.Neighbours
{
    public class Neighbours
    {
        NeighbourListEntry_InviteFriend[] _allInviteFriendEntries;

        //NeighbourListEntry[] _entrylist;
        List<NeigbourFunction> _functions;
        static Random rnd = new Random();

        List<NeighbourListEntry> _entrylist;
        public List<NeighbourListEntry> Entrylist
        {
            get
            {
                if (_allInviteFriendEntries != null && _allInviteFriendEntries.Length > 1)
                {
                    if (LocationOfInviteFriendEntry != Int32.MaxValue)
                    {
                        NeighbourListEntry_InviteFriend newRandom = _allInviteFriendEntries[rnd.Next(_allInviteFriendEntries.Length)];
                        _entrylist[LocationOfInviteFriendEntry] = newRandom;
                    }
                }
                return _entrylist;
            }
        }

        public void SetEntryList(List<NeighbourListEntry> list, List<NeighbourListEntry_InviteFriend> list_inviteFriends)
        {
            _entrylist = list;

            //
            // remove friends who added the app but don't play
            //
            _entrylist.RemoveAll(delegate(NeighbourListEntry temp) { return temp is NeighbourListEntry_PlayingFriend ? System.String.IsNullOrEmpty(((NeighbourListEntry_PlayingFriend)temp).Title) : false; });


            //
            // add generic invite panels depending on how much room we got. 
            //
            if (_entrylist.Count < 6)
            {
                int curCount = _entrylist.Count;
                for (int i = curCount; i < 6; i++ )
                {
                    _entrylist.Add(new NeighbourListEntry_EmptyInvite());
                }
            }
            

            _allInviteFriendEntries = list_inviteFriends.ToArray();
            _locationOfInviteFriendEntry = Int32.MinValue;
        }


        public Neighbours()
        {
        }

        int _locationOfInviteFriendEntry = Int32.MinValue;

        public int LocationOfInviteFriendEntry
        {
            get
            {
                if (_locationOfInviteFriendEntry == Int32.MinValue)
                {
                    _locationOfInviteFriendEntry = Int32.MaxValue; // default value meaning not found
                    for (int i = 0; i < _entrylist.Count; i++)
                    {
                        if (_entrylist[i] is NeighbourListEntry_InviteFriend)
                        {
                            _locationOfInviteFriendEntry = i;
                            continue;
                        }
                    }
                }
                return _locationOfInviteFriendEntry;
            }
        }

        public int NumberOfNeighbours
        {
            get
            {
                int counter = 0;
                foreach (NeighbourListEntry e in _entrylist)
                {
                    if (e is NeighbourListEntry_PlayingFriend)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public void UpdateMe(int xp, int level, string title)
        {
            for (int i = 0; i < _entrylist.Count; i++)
            {
                if (_entrylist[i] is NeighbourListEntry_Me)
                {
                    ((NeighbourListEntry_Me)_entrylist[i]).XP = xp;
                    ((NeighbourListEntry_Me)_entrylist[i]).Level = level;
                    ((NeighbourListEntry_Me)_entrylist[i]).Title = title;
                }
            }
            Entrylist.Sort();
        }

        /// <summary>
        /// if true, it means that list of friends coudl be wrong since we could not get the list of friends playing the game 
        /// from facebook so we used the cached list. 
        /// </summary>
        public bool FailedGettingFriendsListFromFB { get; set; }

        /// <summary>
        /// if true, means that althouth we got the list of friends playing the game from facebook OK, we had trouble getting 
        /// their profiles, meaning that pictures will be wrong
        /// </summary>
        public bool FailedGettingFacebookProfiles { get; set; }
    }



    abstract public class NeighbourListEntry : IComparable
    {
        public NeighbourListEntry()
        {
        }


        public int CompareTo(object obj)
        {
           
                return 0;
           
        }
    }

    public class NeighbourListEntry_EmptyInvite : NeighbourListEntry
    {
        public NeighbourListEntry_EmptyInvite()
            : base()
        {
        }
    }

    
    abstract public class NeighbourListEntry_Person  : NeighbourListEntry
    {
        string _name;
        string _imageUrl;
        string _imageUrlSecure;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public string ImageUrl_Secure
        {
            get { 
                if (String.IsNullOrEmpty(_imageUrlSecure))
                {
                    _imageUrlSecure = _imageUrl.Replace("http://profile.ak.fbcdn.net", "https://fbcdn-profile-a.akamaihd.net"); 
                }
                return _imageUrlSecure; 
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public NeighbourListEntry_Person(string name)
            : base()
        {
            _name = name;
        }
    }

    abstract public class NeighbourListEntry_Friend : NeighbourListEntry_Person
    {        
        public string FriendsName
        {
            get { return Name; }
            set { Name = value; }
        }
        public NeighbourListEntry_Friend(string friendsname)
            : base(friendsname)
        {
        }

    }

    public class NeighbourListEntry_InviteFriend : NeighbourListEntry_Friend
    {
       
        public NeighbourListEntry_InviteFriend(string friendsname)
            : base(friendsname)
        {
        }

    }


    

    public class NeighbourListEntry_PlayingPerson : NeighbourListEntry_Person
    {
        public int Level
        {
            get;
            set;
        }

        public int XP
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        int _playerID=Int32.MinValue;
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; }
        }


        public NeighbourListEntry_PlayingPerson(string name)
            : base(name)
        {
        }
    }

    public class NeighbourListEntry_PlayingFriend : NeighbourListEntry_PlayingPerson
    {
        public string FriendsName
        {
            get { return Name; }
            set { Name = value; }
        }

        public NeighbourListEntry_PlayingFriend(string friendsname)
            : base(friendsname)
        {
        }
    }



    public class NeighbourListEntry_Me : NeighbourListEntry_PlayingPerson
    {
        public NeighbourListEntry_Me(string friendsname)
            : base(friendsname)
        {
        }
    }




    public class NeigbourFunction
    {
        int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}

