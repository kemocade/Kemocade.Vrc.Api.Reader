using Kemocade.Vrc.Api.Reader.Extensions;
using NUnit.Framework;
using System;
using VRC.SDK3.Data;
using static NUnit.Framework.Assert;

namespace Kemocade.Vrc.Api.Reader.Tests
{
    internal class StaticTests
    {
        private const string SAMPLE_JSON =
        @"{
            ""fileTimeUtc"": 130343040000000000,
            ""vrcUserDisplayNames"": [
                ""Lion"",
                ""Tiger"",
                ""Cat"",
                ""Wolf"",
                ""Cow"",
                ""Frog"",
                ""Salamander""
            ],
            ""vrcWorldsById"": {
                ""wrld_7416746a-9a5f-4d49-8135-2ff55388b09d"": {
                    ""name"": ""Furry Karaoke"",
                    ""visits"": 1415496,
                    ""favorites"": 35314,
                    ""occupants"": 109
                }
            },
            ""vrcGroupsById"": {
                ""grp_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"": {
                    ""name"": ""Mammals"",
                    ""vrcUsers"":[0,1,2,3,4],
                    ""roles"": {
                        ""grol_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"": {
                            ""name"": ""King"",
                            ""isAdmin"": true,
                            ""isModerator"": true,
                            ""vrcUsers"": [0]
                        },
                        ""grol_bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"": {
                            ""name"": ""Prince"",
                            ""isModerator"": true,
                            ""vrcUsers"": [3]
                        },
                        ""grol_cccccccc-cccc-cccc-cccc-cccccccccccc"": {
                            ""name"": ""Carnivore"",
                            ""vrcUsers"": [0,1,2,3]
                        }
                    }
                },
                ""grp_bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"": {
                    ""name"": ""Amphibians"",
                    ""vrcUsers"":[5,6],
                    ""roles"": {
                        ""grol_dddddddd-dddd-dddd-dddd-dddddddddddd"": {
                            ""name"": ""Big"",
                            ""isAdmin"": true,
                            ""isModerator"": true,
                            ""vrcUsers"": [5]
                        }
                    }
                }
            },
            ""discordServersById"": {
                ""111111111111111111"": {
                    ""name"": ""Felines"",
                    ""memberCount"": 500,
                    ""vrcUsers"":[0,1,2],
                    ""roles"": {
                        ""222222222222222222"": {
                            ""name"": ""Admin"",
                            ""vrcUsers"": [0],
                            ""isAdmin"": true,
                            ""isModerator"": true
                        },
                        ""333333333333333333"": {
                            ""name"": ""Stripes"",
                            ""vrcUsers"": [1],
                            ""isModerator"": true
                        }
                    }
                },
                ""444444444444444444"": {
                    ""name"": ""Everyone"",
                    ""memberCount"": 1000,
                    ""vrcUsers"":[0,1,2,3,4,5,6],
                    ""roles"": {}
                }
            }
        }";

        private const string SAMPLE_JSON_BAD =
        @"{
            ""qwertyuiop"": [""aaaaa"",],
            ""asdfghjkl"": {
                ""bbbbb"": {
                    ""ccccc"": ""ddddd"",
                    ""eeeee"": ""fffff"",
                }
            }
        }";

        private const string SAMPLE_JSON_BAD_2 =
        @"{
            ""vrcGroupsById"": {
                ""grp_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"": {
                    ""name"": ""Mammals"",
                    ""vrcUsers"":[0,1,2,3,4],
                    ""roles"": {
                        ""grol_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"": {
                            ""name"": ""King"",
                            ""isAdmin"": true,
                            ""isModerator"": true,
                            ""vrcUsers"": [0]
                        }
                    }
                }
            }
        }";

        private const long FILE_TIME_UTC = 130343040000000000;
        private readonly DateTime DATE_TIME = DateTime.FromFileTimeUtc(FILE_TIME_UTC);
        private const string FURRY_KARAOKE = "wrld_7416746a-9a5f-4d49-8135-2ff55388b09d";
        private const string FURRY_KARAOKE_NAME = "Furry Karaoke";
        private const int FURRY_KARAOKE_VISITS = 1415496;
        private const int FURRY_KARAOKE_FAVORITES = 35314;
        private const int FURRY_KARAOKE_OCCUPANTS = 109;
        private const string LION = "Lion";
        private const string TIGER = "Tiger";
        private const string CAT = "Cat";
        private const string WOLF = "Wolf";
        private const string COW = "Cow";
        private const string FROG = "Frog";
        private const string SALAMANDER = "Salamander";
        private const string GROUP_MAMMALS = "grp_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        private const string ROLE_KING = "grol_aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        private const string ROLE_PRINCE = "grol_bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
        private const string ROLE_CARNIVORE = "grol_cccccccc-cccc-cccc-cccc-cccccccccccc";
        private const string GROUP_AMPHIBIANS = "grp_bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
        private const string ROLE_BIG = "grol_dddddddd-dddd-dddd-dddd-dddddddddddd";
        private const string GUILD_FELINES = "111111111111111111";
        private const string ROLE_ADMIN = "222222222222222222";
        private const string ROLE_STRIPES = "333333333333333333";
        private const string GUILD_EVERYONE = "444444444444444444";

        private static DataDictionary Data =>
            VRCJson.TryDeserializeFromJson(SAMPLE_JSON, out DataToken token) ?
            token.DataDictionary : null;

        private static DataDictionary DataBad =>
            VRCJson.TryDeserializeFromJson(SAMPLE_JSON_BAD, out DataToken token) ?
            token.DataDictionary : null;

        private static DataDictionary DataBad2 =>
            VRCJson.TryDeserializeFromJson(SAMPLE_JSON_BAD_2, out DataToken token) ?
            token.DataDictionary : null;

        [Test]
        public void TryGetFileTimeUtc() => True
        (
            Data.TryGetFileTimeUtc(out long fileTimeUtc) &&
            fileTimeUtc == FILE_TIME_UTC
        );

        [Test]
        public void TryGetFileTimeUtcFailBad() =>
            False(DataBad.TryGetFileTimeUtc(out _));

        [Test]
        public void TryGetDateTime() => True
        (
            Data.TryGetDateTime(out DateTime dateTime) &&
            dateTime.Equals(DATE_TIME)
        );

        [Test]
        public void TryGetDateTimeFailBad() =>
            False(DataBad.TryGetDateTime(out _));

        [Test]
        public void TryGetVrcWorldName() => True
        (
            Data.TryGetVrcWorldName(out string name, FURRY_KARAOKE) &&
            name == FURRY_KARAOKE_NAME
        );

        [Test]
        public void TryGetVrcWorldNameFailBad() =>
            False(DataBad.TryGetVrcWorldName(out _, FURRY_KARAOKE));

        [Test]
        public void TryGetVrcWorldVisits() => True
        (
            Data.TryGetVrcWorldVisits(out int visits, FURRY_KARAOKE) &&
            visits == FURRY_KARAOKE_VISITS
        );

        [Test]
        public void TryGetVrcWorldVisitsFailBad() =>
            False(DataBad.TryGetVrcWorldVisits(out _, FURRY_KARAOKE));

        [Test]
        public void TryGetVrcWorldFavorites() => True
        (
            Data.TryGetVrcWorldFavorites(out int favorites, FURRY_KARAOKE) &&
            favorites == FURRY_KARAOKE_FAVORITES
        );

        [Test]
        public void TryGetVrcWorldFavoritesFailBad() =>
            False(DataBad.TryGetVrcWorldFavorites(out _, FURRY_KARAOKE));

        [Test]
        public void TryGetVrcWorldOccupants() => True
        (
            Data.TryGetVrcWorldOccupants(out int occupants, FURRY_KARAOKE) &&
            occupants == FURRY_KARAOKE_OCCUPANTS
        );

        [Test]
        public void TryGetVrcWorldOccupantsFailBad() =>
            False(DataBad.TryGetVrcWorldOccupants(out _, FURRY_KARAOKE));

        [Test]
        public void IsVrcGroupMember() =>
            True(Data.IsVrcGroupMember(LION, GROUP_MAMMALS));

        [Test]
        public void IsVrcGroupMemberFail() =>
            False(Data.IsVrcGroupMember(FROG, GROUP_MAMMALS));

        [Test]
        public void TryGetDisplayNamesFromVrcGroupMembers() => True
        (
            Data.TryGetDisplayNamesFromVrcGroupMembers(out string[] displayNames, GROUP_MAMMALS) &&
            displayNames.Length == 5 &&
            displayNames[0] == LION &&
            displayNames[1] == TIGER &&
            displayNames[2] == CAT &&
            displayNames[3] == WOLF &&
            displayNames[4] == COW
        );

        [Test]
        public void TryGetDisplayNamesFromVrcGroupMembersFail() =>
            False(Data.TryGetDisplayNamesFromVrcGroupMembers(out _, "asdfghjkl"));

        [Test]
        public void TryGetDisplayNamesFromVrcGroupMembersFailBad() =>
            False(DataBad2.TryGetDisplayNamesFromVrcGroupMembers(out _, GROUP_MAMMALS));

        [Test]
        public void IsVrcGroupMemberFailBad() =>
            False(DataBad.IsVrcGroupMember(FROG, GROUP_MAMMALS));

        [Test]
        public void HasVrcGroupRole() =>
            True(Data.HasVrcGroupRole(LION, GROUP_MAMMALS, ROLE_KING));

        [Test]
        public void HasVrcGroupRole2() =>
            True(Data.HasVrcGroupRole(WOLF, GROUP_MAMMALS, ROLE_PRINCE));

        [Test]
        public void HasVrcGroupRoleFail() =>
            False(Data.HasVrcGroupRole(SALAMANDER, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void HasVrcGroupRoleFail2() =>
            False(Data.HasVrcGroupRole(COW, GROUP_MAMMALS, ROLE_CARNIVORE));

        [Test]
        public void HasVrcGroupRoleFailBad() =>
            False(DataBad.HasVrcGroupRole(COW, GROUP_MAMMALS, ROLE_CARNIVORE));

        [Test]
        public void TryGetDisplayNamesFromVrcGroupMembersWithRole() => True
        (
            Data.TryGetDisplayNamesFromVrcGroupMembersWithRole(out string[] displayNames, GROUP_MAMMALS, ROLE_CARNIVORE) &&
            displayNames.Length == 4 &&
            displayNames[0] == LION &&
            displayNames[1] == TIGER &&
            displayNames[2] == CAT &&
            displayNames[3] == WOLF
        );

        [Test]
        public void TryGetDisplayNamesFromVrcGroupMembersWithRoleFail() =>
            False(Data.TryGetDisplayNamesFromVrcGroupMembersWithRole(out _, "asdfghjkl", "zxcvbnm"));

        [Test]
        public void IsVrcGroupAdmin() =>
            True(Data.IsVrcGroupAdmin(FROG, GROUP_AMPHIBIANS));

        [Test]
        public void IsVrcGroupAdminFail() =>
            False(Data.IsVrcGroupAdmin(CAT, GROUP_AMPHIBIANS));

        [Test]
        public void IsVrcGroupAdminFailBad() =>
            False(DataBad.IsVrcGroupAdmin(CAT, GROUP_AMPHIBIANS));

        [Test]
        public void IsVrcGroupModerator() =>
            True(Data.IsVrcGroupModerator(WOLF, GROUP_MAMMALS));

        [Test]
        public void IsVrcGroupModeratorFail() =>
            False(Data.IsVrcGroupModerator(CAT, GROUP_MAMMALS));

        [Test]
        public void IsVrcGroupModeratorFailBad() =>
            False(DataBad.IsVrcGroupModerator(CAT, GROUP_MAMMALS));

        [Test]
        public void IsDiscordMember() =>
            True(Data.IsDiscordMember(COW, GUILD_EVERYONE));

        [Test]
        public void IsDiscordMemberFail() =>
            False(Data.IsDiscordMember(SALAMANDER, GUILD_FELINES));

        [Test]
        public void IsDiscordMemberFailBad() =>
            False(DataBad.IsDiscordMember(SALAMANDER, GUILD_FELINES));

        [Test]
        public void HasDiscordRole() =>
            True(Data.HasDiscordRole(TIGER, GUILD_FELINES, ROLE_STRIPES));

        [Test]
        public void HasDiscordRoleFail() =>
            False(Data.HasDiscordRole(CAT, GUILD_FELINES, ROLE_ADMIN));

        [Test]
        public void HasDiscordRoleFailBad() =>
            False(DataBad.HasDiscordRole(CAT, GUILD_FELINES, ROLE_ADMIN));

        [Test]
        public void TryGetDisplayNamesFromDiscordVrcLinkedMembers() => True
        (
            Data.TryGetDisplayNamesFromDiscordVrcLinkedMembers(out string[] displayNames, GUILD_FELINES) &&
            displayNames.Length == 3 &&
            displayNames[0] == LION &&
            displayNames[1] == TIGER &&
            displayNames[2] == CAT
        );

        [Test]
        public void TryGetDisplayNamesFromDiscordVrcLinkedMembersFail() =>
            False(Data.TryGetDisplayNamesFromDiscordVrcLinkedMembers(out _, "asdfghjkl"));

        [Test]
        public void TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole() => True
        (
            Data.TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole(out string[] displayNames, GUILD_FELINES, ROLE_STRIPES) &&
            displayNames.Length == 1 &&
            displayNames[0] == TIGER
        );


        [Test]
        public void TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRoleFail() =>
            False(Data.TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole(out _, "asdfghjkl", "qwertyuiop"));

        [Test]
        public void IsDiscordAdmin() =>
            True(Data.IsDiscordAdmin(LION, GUILD_FELINES));

        [Test]
        public void IsDiscordAdminFail() =>
            False(Data.IsDiscordAdmin(WOLF, GUILD_FELINES));

        [Test]
        public void IsDiscordAdminFailBad() =>
            False(DataBad.IsDiscordAdmin(WOLF, GUILD_FELINES));

        [Test]
        public void IsDiscordModerator() =>
            True(Data.IsDiscordModerator(LION, GUILD_FELINES));

        [Test]
        public void IsDiscordModeratorFail() =>
            False(Data.IsDiscordModerator(WOLF, GUILD_FELINES));

        [Test]
        public void IsDiscordModeratorFailBad() =>
            False(DataBad.IsDiscordModerator(WOLF, GUILD_FELINES));

        [Test]
        public void IsAdminAnywhere() =>
            True(Data.IsAdminAnywhere(LION));

        [Test]
        public void IsAdminAnywhereFail() =>
            False(Data.IsAdminAnywhere(COW));

        [Test]
        public void IsAdminAnywhereFailBad() =>
            False(DataBad.IsAdminAnywhere(COW));

        [Test]
        public void IsModeratorAnywhere() =>
            True(Data.IsModeratorAnywhere(TIGER));

        [Test]
        public void IsModeratorAnywhereFail() =>
            False(Data.IsModeratorAnywhere(SALAMANDER));

        [Test]
        public void IsModeratorAnywhereFailBad() =>
            False(DataBad.IsModeratorAnywhere(SALAMANDER));

        // Bad Json Data Navigation Tests
        [Test]
        public void TryGetVrcUserDisplayNamesFailBad() =>
            False(DataBad.TryGetVrcUserDisplayNames(out _));

        [Test]
        public void TryGetVrcGroupsByIdFailBad() =>
            False(DataBad.TryGetVrcGroupsById(out _));

        [Test]
        public void TryGetVrcGroupFailBad() =>
            False(DataBad.TryGetVrcGroup(out _, GROUP_AMPHIBIANS));

        [Test]
        public void TryGetVrcGroupName() => True
        (
            Data.TryGetVrcGroupName(out string name, GROUP_AMPHIBIANS) &&
            name == "Amphibians"
        );

        [Test]
        public void TryGetVrcGroupNameFailBad() =>
            False(DataBad.TryGetVrcGroupName(out _, GROUP_AMPHIBIANS));

        [Test]
        public void TryGetVrcGroupMemberCount() => True
        (
            Data.TryGetVrcGroupMemberCount(out int memberCount, GROUP_AMPHIBIANS) &&
            memberCount == 2
        );

        [Test]
        public void TryGetVrcGroupMemberCountFailBad() =>
            False(DataBad.TryGetVrcGroupMemberCount(out _, GROUP_AMPHIBIANS));

        [Test]
        public void TryGetVrcGroupVrcUsersFailBad() =>
            False(DataBad.TryGetVrcGroupVrcUsers(out _, GROUP_AMPHIBIANS));

        [Test]
        public void TryGetVrcGroupRolesFailBad() =>
            False(DataBad.TryGetVrcGroupRoles(out _, GROUP_AMPHIBIANS));

        [Test]
        public void TryGetVrcGroupRoleFailBad() =>
            False(DataBad.TryGetVrcGroupRole(out _, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void TryGetVrcGroupRoleNameFailBad() =>
            False(DataBad.TryGetVrcGroupRoleName(out _, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void TryGetVrcGroupRoleName() =>
            True
            (
                Data.TryGetVrcGroupRoleName(out string name, GROUP_AMPHIBIANS, ROLE_BIG) &&
                name == "Big"
            );

        [Test]
        public void TryGetVrcGroupRoleIsAdminFailBad() =>
            False(DataBad.TryGetVrcGroupRoleIsAdmin(out _, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void TryGetVrcGroupRoleIsModeratorFailBad() =>
            False(DataBad.TryGetVrcGroupRoleIsModerator(out _, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void TryGetVrcGroupRoleVrcUsersFailBad() =>
            False(DataBad.TryGetVrcGroupRoleVrcUsers(out _, GROUP_AMPHIBIANS, ROLE_BIG));

        [Test]
        public void TryGetDiscordServersByIdFailBad() =>
            False(DataBad.TryGetDiscordServersById(out _));

        [Test]
        public void TryGetDiscordServerFailBad() =>
            False(DataBad.TryGetDiscordServer(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordServerNameFailBad() =>
            False(DataBad.TryGetDiscordServerName(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordServerName() => True
        (
            Data.TryGetDiscordServerName(out string name, GUILD_EVERYONE) &&
            name == "Everyone"
        );

        [Test]
        public void TryGetDiscordServerVrcUsersFailBad() =>
            False(DataBad.TryGetDiscordServerVrcUsers(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordServerMemberCount() => True
        (
            Data.TryGetDiscordServerMemberCount(out int memberCount, GUILD_EVERYONE) &&
            memberCount == 1000
        );

        [Test]
        public void TryGetDiscordServerMemberCountFailBad() =>
            False(DataBad.TryGetDiscordServerMemberCount(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordVrcLinkedMemberCount() => True
        (
            Data.TryGetDiscordVrcLinkedMemberCount(out int vrcLinkedMemberCount, GUILD_EVERYONE) &&
            vrcLinkedMemberCount == 7
        );

        [Test]
        public void TryGetDiscordVrcLinkedMemberCountFailBad() =>
            False(DataBad.TryGetDiscordVrcLinkedMemberCount(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordServerRolesFailBad() =>
            False(DataBad.TryGetDiscordServerRoles(out _, GUILD_FELINES));

        [Test]
        public void TryGetDiscordServerRoleFailBad() =>
            False(DataBad.TryGetDiscordServerRole(out _, GUILD_FELINES, ROLE_STRIPES));

        [Test]
        public void TryGetDiscordServerRoleNameFailBad() =>
            False(DataBad.TryGetDiscordServerRoleName(out _, GUILD_FELINES, ROLE_STRIPES));

        [Test]
        public void TryGetDiscordServerRoleName() =>
            True
            (
                Data.TryGetDiscordServerRoleName(out string name, GUILD_FELINES, ROLE_STRIPES) &&
                name == "Stripes"
            );

        [Test]
        public void TryGetDiscordServerRoleIsAdminFailBad() =>
            False(DataBad.TryGetDiscordServerRoleIsAdmin(out _, GUILD_FELINES, ROLE_STRIPES));

        [Test]
        public void TryGetDiscordServerRoleIsModeratorFailBad() =>
            False(DataBad.TryGetDiscordServerRoleIsModerator(out _, GUILD_FELINES, ROLE_STRIPES));

        [Test]
        public void TryGetDiscordServerRoleVrcUsersFailBad() =>
            False(DataBad.TryGetDiscordServerRoleVrcUsers(out _, GUILD_FELINES, ROLE_STRIPES));
    }
}
