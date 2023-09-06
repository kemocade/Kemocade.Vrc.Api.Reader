using Kemocade.Vrc.Api.Reader.Extensions;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using static UnityEngine.Debug;
using static VRC.SDKBase.Networking;

namespace Kemocade.Vrc.Api.Reader
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Reader : UdonSharpBehaviour
    {
        [SerializeField] private VRCUrl _dataUrl;
        [SerializeField] private bool _allowAutoRefresh = true;
        [SerializeField] private double _hoursToWaitAfterUpdate = 1;
        [SerializeField] private double _minutesToWaitBetweenRefreshes = 5;
        private TimeSpan _timeToWaitAfterUpdate;
        private TimeSpan _timeToWaitBetweenRefreshes;
        private DataDictionary _dictionary;
        private DateTime _lastUpdate;
        private DateTime _lastRefresh;
        private bool _refreshing = false;

        // Unity Messages
        protected void Start()
        {
            _timeToWaitAfterUpdate = TimeSpan.FromHours(_hoursToWaitAfterUpdate);
            _timeToWaitBetweenRefreshes = TimeSpan.FromMinutes(_minutesToWaitBetweenRefreshes);
            Refresh();
        } 

        protected void FixedUpdate()
        {
            // If auto refresh is disabled or no update or refresh has occured, skip
            if (!_allowAutoRefresh || _lastUpdate == default || _lastRefresh == default)
            { return; }

            // If sufficient time has not yet passed since the last Update, wait
            DateTime networkDateTime = GetNetworkDateTime();
            TimeSpan timeSinceLastUpdate = networkDateTime - _lastUpdate;
            if (timeSinceLastUpdate < _timeToWaitAfterUpdate) { return; }

            // If sufficient time has not yet passed since the last Refresh, wait
            TimeSpan timeSinceLastRefresh = networkDateTime - _lastRefresh;
            if ( timeSinceLastRefresh < _timeToWaitBetweenRefreshes ) { return; }

            Refresh();
        }

        // Udon Events
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            string json = result.Result;
            if (!VRCJson.TryDeserializeFromJson(json, out DataToken token))
            {
                LogError($"Failed to Deserialize JSON {json} - {result}");
                _refreshing = false;
                return;
            }

            if (token.TokenType != TokenType.DataDictionary)
            {
                LogError($"JSON was not a DataDictionary.");
                _refreshing = false;
                return;
            }
            _dictionary = token.DataDictionary;

            if (!TryGetDateTime(out _lastUpdate))
            {
                LogError($"Failed to parse DateTime from JSON 'fileTimeUtc'.");
                _refreshing = false;
                return;
            }

            _lastRefresh = GetNetworkDateTime();
            _refreshing = false;
            OnRefresh();
        }

        public override void OnStringLoadError(IVRCStringDownload result) =>
            LogError($"String Load Error ({result.ErrorCode}): {result.Error}");

        // Callbacks
        public virtual void OnRefresh() { }

        // Manual Refresh
        /// <summary>
        /// Manually force the remote data to be refreshed
        /// </summary>
        /// <remarks>
        /// Calls to this method will be ignored if a Refresh is already in progress
        /// </remarks>
        public void Refresh()
        {
            if (_refreshing) { return; }
            _refreshing = true;

            VRCStringDownloader.LoadUrl(_dataUrl, (IUdonEventReceiver)this);
        }

        // Accessors
        /// <summary>
        /// Whether or not remote data has successfully been downloaded at least one time and is ready to be accessed
        /// </summary>
        /// <returns>
        /// True if the Reader is ready to parse downloaded data
        /// </returns>
        public bool IsReady => _dictionary != null;

        // Data Checks
        // Metadata
        /// <summary>
        /// Try to extract a UTC File Time from the downloaded data
        /// </summary>
        /// <param name="fileTimeUtc">Output UTC File Time (if valid)</param>
        /// <returns>True if a valid UTC File Time could be found in the downloaded data</returns>
        public bool TryGetFileTimeUtc(out long fileTimeUtc) =>
            _dictionary.TryGetFileTimeUtc(out fileTimeUtc);

        /// <summary>
        /// Try to extract a DateTime from the downloaded data
        /// </summary>
        /// <param name="dateTime">Output DateTime (if valid)</param>
        /// <returns>True if a valid DateTime could be parsed from the downloaded data</returns>
        public bool TryGetDateTime(out DateTime dateTime) =>
            _dictionary.TryGetDateTime(out dateTime);

        // VRC World Data
        /// <summary>
        /// Try to extract a VRC World name from the downloaded data
        /// </summary>
        /// <param name="name">Output name (if valid)</param>
        /// <param name="vrcWorldId">The VRC World ID of the target World</param>
        /// <returns>True if a valid name could be found for the given VRC World</returns>
        public bool TryGetVrcWorldName(out string name, string vrcWorldId) =>
            _dictionary.TryGetVrcWorldName(out name, vrcWorldId);

        /// <summary>
        /// Try to extract a VRC World visits count from the downloaded data
        /// </summary>
        /// <param name="visits">Output visits count (if valid)</param>
        /// <param name="vrcWorldId">The VRC World ID of the target World</param>
        /// <returns>True if a valid visits count could be found for the given VRC World</returns>
        public bool TryGetVrcWorldVisits(out int visits, string vrcWorldId) =>
            _dictionary.TryGetVrcWorldVisits(out visits, vrcWorldId);

        /// <summary>
        /// Try to extract a VRC World favorites count from the downloaded data
        /// </summary>
        /// <param name="favorites">Output favorites count (if valid)</param>
        /// <param name="vrcWorldId">The VRC World ID of the target World</param>
        /// <returns>True if a valid favorites count could be found for the given VRC World</returns>
        public bool TryGetVrcWorldFavorites(out int favorites, string vrcWorldId) =>
            _dictionary.TryGetVrcWorldFavorites(out favorites, vrcWorldId);

        /// <summary>
        /// Try to extract a VRC World occupants count from the downloaded data
        /// </summary>
        /// <param name="occupants">Output occupants count (if valid)</param>
        /// <param name="vrcWorldId">The VRC World ID of the target World</param>
        /// <returns>True if a valid occupants count could be found for the given VRC World</returns>
        public bool TryGetVrcWorldOccupants(out int occupants, string vrcWorldId) =>
            _dictionary.TryGetVrcWorldOccupants(out occupants, vrcWorldId);

        // VRC Group Data
        /// <summary>
        /// Try to extract a VRC Group name from the downloaded data
        /// </summary>
        /// <param name="name">Output name (if valid)</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target Group</param>
        /// <returns>True if a valid name could be found for the given VRC Group</returns>
        public bool TryGetVrcGroupName(out string name, string vrcGroupId) =>
            _dictionary.TryGetVrcGroupName(out name, vrcGroupId);

        /// <summary>
        /// Try to extract a VRC Group member count from the downloaded data
        /// </summary>
        /// <param name="memberCount">Output member count (if valid)</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target Group</param>
        /// <returns>True if a valid member count could be found for the given VRC Group</returns>
        public bool TryGetVrcGroupMemberCount(out int memberCount, string vrcGroupId) =>
            _dictionary.TryGetVrcGroupMemberCount(out memberCount, vrcGroupId);

        /// <summary>
        /// Try to extract VRC Group member display names from the downloaded data
        /// </summary>
        /// <param name="displayNames">Output display names (if valid)</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target Group</param>
        /// <returns>True if valid display names could be found for the given VRC Group</returns>
        public bool TryGetDisplayNamesFromVrcGroupMembers(out string[] displayNames, string vrcGroupId) =>
            _dictionary.TryGetDisplayNamesFromVrcGroupMembers(out displayNames, vrcGroupId);

        /// <summary>
        /// Try to extract display names of VRC Group member with a given role from the downloaded data
        /// </summary>
        /// <param name="displayNames">Output display names (if valid)</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target Group</param>
        /// <param name="vrcGroupRoleId">The VRC Group Role ID of the target Group Role</param>
        /// <returns>True if valid display names could be found for the given role in the given VRC Group</returns>
        public bool TryGetDisplayNamesFromVrcGroupMembersWithRole(out string[] displayNames, string vrcGroupId, string vrcGroupRoleId) =>
            _dictionary.TryGetDisplayNamesFromVrcGroupMembersWithRole(out displayNames, vrcGroupId, vrcGroupRoleId);

        // VRC Group Member Data by Display Name
        /// <summary>
        /// Check if the given User is a member of the given VRC Group
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the given user is a member of the given VRC Group</returns>
        public bool IsVrcGroupMember(string name, string vrcGroupId) =>
            _dictionary.IsVrcGroupMember(name, vrcGroupId);

        /// <summary>
        /// Check if the given user has the given role in the given VRC Group
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <param name="vrcGroupRoleId">The VRC Group Role ID of the target VRC Group Role</param>
        /// <returns>True if the given user has the given role in the given VRC Group</returns>
        public bool HasVrcGroupRole(string name, string vrcGroupId, string vrcGroupRoleId) =>
            _dictionary.HasVrcGroupRole(name, vrcGroupId, vrcGroupRoleId);

        /// <summary>
        /// Check if the given user is the owner of the given VRC Group
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the given user is the owner of the given VRC Group</returns>
        public bool IsVrcGroupAdmin(string name, string vrcGroupId) =>
            _dictionary.IsVrcGroupAdmin(name, vrcGroupId);

        /// <summary>
        /// Check if the given user has any role with instance moderation permissions in the VRC Group
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the given user has any role with instance moderation permissions in the VRC Group</returns>
        public bool IsVrcGroupModerator(string name, string vrcGroupId) =>
            _dictionary.IsVrcGroupModerator(name, vrcGroupId);

        // VRC Group Member Data for Local User
        /// <summary>
        /// Check if the local User is a member of the given VRC Group
        /// </summary>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the local user is a member of the given VRC Group</returns>
        public bool IsVrcGroupMemberLocal(string vrcGroupId) =>
            TryGetLocalPlayerName(out string name) &&
            IsVrcGroupMember(name, vrcGroupId);

        /// <summary>
        /// Check if the local user has the given role in the given VRC Group
        /// </summary>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <param name="vrcGroupRoleId">The VRC Group Role ID of the target VRC Group Role</param>
        /// <returns>True if the local user has the given role in the given VRC Group</returns>
        public bool HasVrcGroupRoleLocal(string vrcGroupId, string vrcGroupRoleId) =>
            TryGetLocalPlayerName(out string name) &&
            HasVrcGroupRole(name, vrcGroupId, vrcGroupRoleId);

        /// <summary>
        /// Check if the local user is the owner of the given VRC Group
        /// </summary>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the local user is the owner of the given VRC Group</returns>
        public bool IsVrcGroupAdminLocal(string vrcGroupId) =>
            TryGetLocalPlayerName(out string name) &&
            IsVrcGroupAdmin(name, vrcGroupId);

        /// <summary>
        /// Check if the local user has any role with instance moderation permissions in the VRC Group
        /// </summary>
        /// <param name="vrcGroupId">The VRC Group ID of the target VRC Group</param>
        /// <returns>True if the local user has any role with instance moderation permissions in the VRC Group</returns>
        public bool IsVrcGroupModeratorLocal(string vrcGroupId) =>
            TryGetLocalPlayerName(out string name) &&
            IsVrcGroupModerator(name, vrcGroupId);

        // Discord Server Data
        /// <summary>
        /// Try to extract a Discord Server name from the downloaded data
        /// </summary>
        /// <param name="name">Output name (if valid)</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if a valid name could be found for the given Discord Server</returns>
        public bool TryGetDiscordServerName
        (out string name, string discordGuildId) =>
            _dictionary.TryGetDiscordServerName(out name, discordGuildId);

        /// <summary>
        /// Try to extract a Discord Server member count from the downloaded data
        /// </summary>
        /// <param name="memberCount">Output member count (if valid)</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if a valid member count could be found for the given Discord Server</returns>
        /// <remarks>This count includes all Discord Server members, even those without linked VRChat accounts</remarks>
        public bool TryGetDiscordServerMemberCount
        (out int memberCount, string discordGuildId) =>
            _dictionary.TryGetDiscordServerMemberCount(out memberCount, discordGuildId);

        /// <summary>
        /// Try to extract a valid member count for the given Discord Server's members who have been linked with VRC users from the downloaded data
        /// </summary>
        /// <param name="vrcLinkedMemberCount">Output member count (if valid)</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if a valid member count could be found for the given Discord Server</returns>
        /// <remarks>This count only includes Discord Server members with linked VRChat accounts</remarks>
        public bool TryGetDiscordVrcLinkedMemberCount
        (out int vrcLinkedMemberCount, string discordGuildId) =>
            _dictionary.TryGetDiscordVrcLinkedMemberCount(out vrcLinkedMemberCount, discordGuildId);

        /// <summary>
        /// Try to extract VRC display names from the given Discord Server's members who have been linked with VRC users from the downloaded data
        /// </summary>
        /// <param name="displayNames">Output VRC display names (if valid)</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if valid VRC display names could be found for the given Discord Server's members who have been linked with VRC users</returns>
        public bool TryGetDisplayNamesFromDiscordVrcLinkedMembers
        (out string[] displayNames, string discordGuildId) =>
            _dictionary.TryGetDisplayNamesFromDiscordVrcLinkedMembers(out displayNames, discordGuildId);

        /// <summary>
        /// Try to extract VRC display names from the given Discord Server's members who have been linked with VRC users and have a particular role from the downloaded data
        /// </summary>
        /// <param name="displayNames">Output VRC display names (if valid)</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <param name="discordRoleId">The Discord Role ID of the target Discord Role</param>
        /// <returns>True if valid displayNames could be found for the given role in the given Discord Server's members who have been linked with VRC users</returns>
        public bool TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole
        (out string[] displayNames, string discordGuildId, string discordRoleId) =>
            _dictionary.TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole(out displayNames, discordGuildId, discordRoleId);

        // Discord Server Member Data by Display Name
        /// <summary>
        /// Check if the given user is a member of the given Discord Server
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the given user is a member of the given Discord Server</returns>
        public bool IsDiscordMember
        (string name, string discordGuildId) =>
            _dictionary.IsDiscordMember(name, discordGuildId);

        /// <summary>
        /// Check if the given user has the given role in the given Discord Server
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <param name="discordRoleId">The Discord Role ID of the target Discord Role</param>
        /// <returns>True if the given user has the given role in the given Discord Server</returns>
        public bool HasDiscordRole
        (string name, string discordGuildId, string discordRoleId) =>
            _dictionary.HasDiscordRole(name, discordGuildId, discordRoleId);

        /// <summary>
        /// Check if the given user has any role with Admin permissions in the given Discord Server
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the given user has any role with Admin permissions in the given Discord Server</returns>
        public bool IsDiscordAdmin(string name, string discordGuildId) =>
            _dictionary.IsDiscordAdmin(name, discordGuildId);

        /// <summary>
        /// Check if the given user has any role with Moderation permissions in the given Discord Server
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the given user has any role with Moderation permissions in the given Discord Server</returns>
        public bool IsDiscordModerator(string name, string discordGuildId) =>
            _dictionary.IsDiscordModerator(name, discordGuildId);

        // Discord Server Member Data for Local User
        /// <summary>
        /// Check if the local user is a member of the given Discord Server
        /// </summary>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the local user is a member of the given Discord Server</returns>
        public bool IsDiscordMemberLocal(string discordGuildId) =>
            TryGetLocalPlayerName(out string name) &&
            IsDiscordMember(name, discordGuildId);

        /// <summary>
        /// Check if the local user has the given role in the given Discord Server
        /// </summary>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <param name="discordRoleId">The Discord Role ID of the target Discord Role</param>
        /// <returns>True if the local user has the given role in the given Discord Server</returns>
        public bool HasDiscordRoleLocal(string discordGuildId, string discordRoleId) =>
            TryGetLocalPlayerName(out string name) &&
            HasDiscordRole(name, discordGuildId, discordRoleId);

        /// <summary>
        /// Check if the local user has any role with Admin permissions in the given Discord Server
        /// </summary>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the local user has any role with Admin permissions in the given Discord Server</returns>
        public bool IsDiscordAdminLocal(string discordGuildId) =>
            TryGetLocalPlayerName(out string name) &&
            IsDiscordAdmin(name, discordGuildId);

        /// <summary>
        /// Check if the local user has any role with Moderation permissions in the given Discord Server
        /// </summary>
        /// <param name="discordGuildId">The Discord Guild ID of the target Discord Server</param>
        /// <returns>True if the local user has any role with Moderation permissions in the given Discord Server</returns>
        public bool IsDiscordModeratorLocal(string discordGuildId) =>
            TryGetLocalPlayerName(out string name) &&
            IsDiscordModerator(name, discordGuildId);

        // Combined Data by Display Name
        /// <summary>
        /// Check if the given user has any role with Admin permissions across any tracked VRC Groups or Discord Servers
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <returns>True if the given user has any role with Admin permissions across any tracked VRC Groups or Discord Servers</returns>
        public bool IsAdminAnywhere(string name) =>
            _dictionary.IsAdminAnywhere(name);

        /// <summary>
        /// Check if the given user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers
        /// </summary>
        /// <param name="name">The VRC display name of the target User</param>
        /// <returns>True if the given user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers</returns>
        public bool IsModeratorAnywhere(string name) =>
            _dictionary.IsModeratorAnywhere(name);

        // Combined Data for Local User
        /// <summary>
        /// Check if the local user has any role with Admin permissions across any tracked VRC Groups or Discord Servers
        /// </summary>
        /// <returns>True if the local user has any role with Admin permissions across any tracked VRC Groups or Discord Servers</returns>
        public bool IsAdminAnywhereLocal() =>
            TryGetLocalPlayerName(out string name) &&
            IsAdminAnywhere(name);

        /// <summary>
        /// Check if the local user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers
        /// </summary>
        /// <returns>True if the local user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers</returns>
        public bool IsModeratorAnywhereLocal() =>
            TryGetLocalPlayerName(out string name) &&
            IsModeratorAnywhere(name);

        // Safe check to get local player name without edge-case exceptions
        private bool TryGetLocalPlayerName(out string name)
        {
            if
            (
                LocalPlayer == null ||
                !LocalPlayer.IsValid() ||
                string.IsNullOrEmpty(name = LocalPlayer.displayName)
            )
            {
                name = null;
                return false;
            }

            return true;
        }
    }
}