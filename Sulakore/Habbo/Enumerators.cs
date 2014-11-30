﻿namespace Sulakore.Habbo
{
    /// <summary>
    /// Specifies the different types of bans found in-game.
    /// </summary>
    public enum HBans
    {
        /// <summary>
        /// The original value for the packet is RWUAM_BAN_USER_DAY.
        /// </summary>
        Hour = 0,
        /// <summary>
        /// The original value for the packet is RWUAM_BAN_USER_HOUR.
        /// </summary>
        Day = 1,
        /// <summary>
        /// The original value for the packet is RWUAM_BAN_USER_PERM.
        /// </summary>
        Permanent = 2
    }

    /// <summary>
    /// Specifies the different types of dances your player can perform in-game.
    /// </summary>
    public enum HDances
    {
        /// <summary>
        /// Represents a non-dancing player.
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents the default dance any player can perform.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Represents the duck funk dance. (HC Only)
        /// </summary>
        PogoMogo = 2,
        /// <summary>
        /// Represents the pogo mogo dance. (HC Only).
        /// </summary>
        DuckFunk = 3,
        /// <summary>
        /// Represents the rollie dance. (HC Only)
        /// </summary>
        TheRollie = 4
    }

    /// <summary>
    /// Specifies the compass locations of an object/player in-game.
    /// </summary>
    public enum HDirections
    {
        /// <summary>
        /// Represents an object/player facing north.
        /// </summary>
        North = 0,
        /// <summary>
        /// Represents an object/player facing north east.
        /// </summary>
        NorthEast = 1,
        /// <summary>
        /// Represents an object/player facing east.
        /// </summary>
        East = 2,
        /// <summary>
        /// Represents an object/player facing south east.
        /// </summary>
        SouthEast = 3,
        /// <summary>
        /// Represents an object/player facing south.
        /// </summary>
        South = 4,
        /// <summary>
        /// Represents an object/player facing south west.
        /// </summary>
        SouthWest = 5,
        /// <summary>
        /// Represents an object/player facing west.
        /// </summary>
        West = 6,
        /// <summary>
        /// Represents an object/player facing north west.
        /// </summary>
        NorthWest = 7
    }

    /// <summary>
    /// Specifies the different gender types in-game.
    /// </summary>
    public enum HGenders
    {
        Unknown = 0,
        /// <summary>
        /// Represents a male player.
        /// </summary>
        Male = 'M',
        /// <summary>
        /// Represents a female player.
        /// </summary>
        Female = 'F'
    }

    /// <summary>
    /// Specifies a set of gestures/actions a player can perform in-game.
    /// </summary>
    public enum HGestures
    {
        /// <summary>
        /// Represents a player without any gesture.
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents a player waving.
        /// </summary>
        Wave = 1,
        /// <summary>
        /// Represents a player blowing a kiss. (HC Only)
        /// </summary>
        BlowKiss = 2,
        /// <summary>
        /// Represents a player laughing. (HC Only)
        /// </summary>
        Laugh = 3,
        /// <summary>
        /// Represents a player sleeping.
        /// </summary>
        Idle = 5,
        /// <summary>
        /// Represents a player hopping once. (HC Only)
        /// </summary>
        PogoHop = 6,
        /// <summary>
        /// Represents a player raising a thumbs up.
        /// </summary>
        ThumbsUp = 7
    }

    /// <summary>
    /// Specifies the different types of room models you can create in-game.
    /// </summary>
    public enum HModels
    {
        /// <summary>
        /// Represents a room with 104 tiles(8x13).
        /// </summary>
        A = 'a',
        /// <summary>
        /// Represents a room with 94 tiles((11x10) - (4x4)).
        /// </summary>
        B = 'b',
        /// <summary>
        /// Represents a room with 36 tiles(6x6).
        /// </summary>
        C = 'c',
        /// <summary>
        /// Represents a room with 84 tiles(6x14).
        /// </summary>
        D = 'd',
        /// <summary>
        /// Represents a room with 80 tiles(10x8).
        /// </summary>
        E = 'e',
        /// <summary>
        /// Represents a room with 84 tiles((10x10) - ((2x4)x2))
        /// </summary>
        F = 'f',
        /// <summary>
        /// Represents a room with 416 tiles(16x26).
        /// </summary>
        I = 'i',
        /// <summary>
        /// Represents a room with 380 tiles((20x22) - (10x6)).
        /// </summary>
        J = 'j',
        /// <summary>
        /// Represents a room with 448 tiles(((24x26) - (8x10)) - ((8x4)x3)).
        /// </summary>
        K = 'k',
        /// <summary>
        /// Represents a room with 352 tiles((20x20) - (4x12))
        /// </summary>
        L = 'l',
        /// <summary>
        /// Represents a room with 704 tiles((28x28) - (10x8))
        /// </summary>
        M = 'm',
        /// <summary>
        /// Represents a room with 368 tiles((20x20) - (8x4)).
        /// </summary>
        N = 'n',
        /// <summary>
        /// Represents a room with 35 tiles(5x7).
        /// </summary>
        S = 's'
    }

    /// <summary>
    /// Specifies the set of pages that are compatible with Sulakore.HSession[HPages].
    /// </summary>
    public enum HPages
    {
        /// <summary>
        /// Represents http://www.Habbo-/me
        /// </summary>
        Me = 1,
        /// <summary>
        /// Represents http://www.Habbo-/home/-
        /// </summary>
        Home = 2,
        /// <summary>
        /// Represents http://www.Habbo-/client
        /// </summary>
        Client = 3,
        /// <summary>
        /// Represents http://www.Habbo-/profile
        /// </summary>
        Profile = 4,
        /// <summary>
        /// Represents https://www.Habbo-/identity/avatars
        /// </summary>
        IdAvatars = 5,
        /// <summary>
        /// Represents http://www.Habbo-/identity/settings
        /// </summary>
        IdSettings = 6
    }

    /// <summary>
    /// Specifies the different types of signs found in-game.
    /// </summary>
    public enum HSigns
    {
        /// <summary>
        /// Represents a random sign within the range of (0 - 18)
        /// </summary>
        Random = -1,
        /// <summary>
        /// Represents a sign with an image of the number zero.
        /// </summary>
        Zero = 0,
        /// <summary>
        /// Represents a sign with an image of the number one.
        /// </summary>
        One = 1,
        /// <summary>
        /// Represents a sign with an image of the number two.
        /// </summary>
        Two = 2,
        /// <summary>
        /// Represents a sign with an image of the number three.
        /// </summary>
        Three = 3,
        /// <summary>
        /// Represents a sign with an image of the number four.
        /// </summary>
        Four = 4,
        /// <summary>
        /// Represents a sign with an image of the number five.
        /// </summary>
        Five = 5,
        /// <summary>
        /// Represents a sign with an image of the number six.
        /// </summary>
        Six = 6,
        /// <summary>
        /// Represents a sign with an image of the number seven.
        /// </summary>
        Seven = 7,
        /// <summary>
        /// Represents a sign with an image of the number eight.
        /// </summary>
        Eight = 8,
        /// <summary>
        /// Represents a sign with an image of the number nine.
        /// </summary>
        Nine = 9,
        /// <summary>
        /// Represents a sign with an image of the number ten.
        /// </summary>
        Ten = 10,
        /// <summary>
        /// Represents a sign with an image of a heart.
        /// </summary>
        Heart = 11,
        /// <summary>
        /// Represents a sign with an image of a skull.
        /// </summary>
        Skull = 12,
        /// <summary>
        /// Represents a sign with an image of a exclamation mark.
        /// </summary>
        Exclamation = 13,
        /// <summary>
        /// Represents a sign with an image of a soccerball.
        /// </summary>
        Soccerball = 14,
        /// <summary>
        /// Represents a sign with an image of a smiley face.
        /// </summary>
        Smiley = 15,
        /// <summary>
        /// Represents a sign with an image of a small redcard.
        /// </summary>
        Redcard = 16,
        /// <summary>
        /// Represents a sign with an image of a small yellowcard.
        /// </summary>
        Yellowcard = 17,
        /// <summary>
        /// Represents a an empty invisible sign with no image.
        /// </summary>
        Invisible = 18
    }

    /// <summary>
    /// Specifies the state of a player.
    /// </summary>
    public enum HStances
    {
        /// <summary>
        /// Represents a player that is standing.
        /// </summary>
        Stand = 0,
        /// <summary>
        /// Represents a player that is sitting.
        /// </summary>
        Sit = 1
    }

    /// <summary>
    /// Specifies the different types of speech bubble themes found in-game.
    /// </summary>
    public enum HThemes
    {
        /// <summary>
        /// Represents a random speech bubble consisting of (White, Red, Blue, Yellow, Green, Black).
        /// </summary>
        Random = -1,
        /// <summary>
        /// Represents the default(white) speech bubble.
        /// </summary>
        White = 0,
        /// <summary>
        /// Represents the red speech bubble.
        /// </summary>
        Red = 3,
        /// <summary>
        /// Represents the blue speech bubble.
        /// </summary>
        Blue = 4,
        /// <summary>
        /// Represents the yellow speech bubble.
        /// </summary>
        /// 
        Yellow = 5,
        /// <summary>
        /// Represents the green speech bubble.
        /// </summary>
        Green = 6,
        /// <summary>
        /// Represents the black speech bubble.
        /// </summary>
        Black = 7,
        /// <summary>
        /// Represents the light-blue(ice) speech bubble.
        /// </summary>
        Ice = 11,
        /// <summary>
        /// Represents the pink speech bubble.
        /// </summary>
        Pink = 12,
        /// <summary>
        /// Represents the purple speech bubble.
        /// </summary>
        Purple = 13,
        /// <summary>
        /// Represents the gold speech bubble.
        /// </summary>
        Gold = 14,
        /// <summary>
        /// Represents the turquoise speech bubble.
        /// </summary>
        Turquoise = 15,
        /// <summary>
        /// Represents a speech bubble with hearts in the background.
        /// </summary>
        Hearts = 16,
        /// <summary>
        /// Represents a speech bubble with roses in the background.
        /// </summary>
        Roses = 17,
        /// <summary>
        /// Represents a speech bubble with a pig in the background.
        /// </summary>
        Pig = 19,
        /// <summary>
        /// Represents a speech bubble with a dog in the background.
        /// </summary>
        Dog = 20,
        /// <summary>
        /// Represents a speech bubble with swords in the background.
        /// </summary>
        Swords = 29
    }

    /// <summary>
    /// Specifies the different types of speech modes that players can communicate with each other in-game.
    /// </summary>
    public enum HSpeeches
    {
        /// <summary>
        /// Represents a speech mode that makes the message publicly visible within a determined range in the room.
        /// </summary>
        Say = 0,
        /// <summary>
        /// Represents a speech mode that makes the message public to everyone in the room.
        /// </summary>
        Shout = 1,
        /// <summary>
        /// Represents a speech mode that only makes the message visible to the specified target regardless of the range.
        /// </summary>
        Whisper = 2
    }

    /// <summary>
    /// Specifies the different types of choices given on the page represented by Sulakore.HPages.Profile.
    /// </summary>
    public enum HVisibility
    {
        Unknown = 0,
        /// <summary>
        /// Represents a value in-which the object being referenced has zero visibility to the public.
        /// </summary>
        Nobody = 1,
        /// <summary>
        /// Represents a value in-which the object being referenced allows the public's view.
        /// </summary>
        Everybody = 2,
        /// <summary>
        /// Represents a value in-which the object being referenced only allows to been seen by friends.
        /// </summary>
        MyFriends = 3
    }
}