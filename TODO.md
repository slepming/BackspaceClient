# General
 - [X] Log in
 - [X] Sidebar with users and avatars;
 - [X] Sidebar search container;
 - [ ] Chat with user;
 - [ ] Groups chats and sidebar update;
 - [ ] Instance profiles;
 - [ ] Transfer of data between instances;
 - [ ] Registration

# UI

## Avatars
 - [ ] Convert webp in PNG. Cuz texture couldn't be loaded via STB

## Chat
 - [ ] Chat messages list
    - [X] Print messages
    - [X] Always when user get in chat, chat have to scroll messages to end
    - [ ] List messages on scroll
 - [X] Enter messages

## Instance movement
 - [ ] Instances list in MessengerScreen
 - [ ] Registration between instances

## Menus
 - [ ] Profile menu
 - [ ] Settings menu
 - [ ] Instances menu
 - [ ] Keybinds menu

# Network

## REST API

### Auth
- [ ] `POST /auth/register`
- [ ] `GET  /auth/check-username?username=`
- [ ] `GET  /auth/check-invite?token=`
- [X] `POST /auth/login`
- [ ] `POST /auth/attach-proof`

### Users
- [ ] `GET    /users/@me`
- [ ] `PATCH  /users/@me`
- [ ] `POST   /users/@me/verify-password`
- [ ] `POST   /users/@me/change-password`
- [ ] `DELETE /users/@me`
- [ ] `PUT    /users/@me/space-layout`
- [ ] `GET    /users/@me/federation-registry`
- [ ] `PUT    /users/@me/federation-registry`
- [ ] `POST   /users/@me/federation-credential`
- [ ] `POST   /users/@me/reattach`
- [ ] `POST   /users/@me/federation-identity/delete`
- [ ] `GET    /users/@me/join-requests`
- [ ] `GET    /users/:id`
- [ ] `GET    /users/:id/mutuals`

### Spaces
- [ ] `GET    /spaces`
- [ ] `POST   /spaces`
- [ ] `GET    /spaces/:id`
- [ ] `PATCH  /spaces/:id`
- [ ] `DELETE /spaces/:id`
- [ ] `POST   /spaces/:id/invite`
- [ ] `POST   /spaces/:id/join`
- [ ] `POST   /spaces/join`
- [ ] `GET    /spaces/invite/:code/preview`
- [ ] `PATCH  /spaces/:id/transfer-ownership`

### Space Members
- [ ] `GET    /spaces/:id/members`
- [ ] `PATCH  /spaces/:id/members/:uid`
- [ ] `DELETE /spaces/:id/members/:uid`

### Bans
- [ ] `GET    /spaces/:id/bans`
- [ ] `POST   /spaces/:id/bans`
- [ ] `DELETE /spaces/:id/bans/:uid`

### Roles
- [ ] `POST   /spaces/:id/roles`
- [ ] `PATCH  /spaces/:id/roles/:rid`
- [ ] `DELETE /spaces/:id/roles/:rid`
- [ ] `POST   /spaces/:id/members/:uid/roles`
- [ ] `DELETE /spaces/:id/members/:uid/roles/:rid`

### Channels
- [ ] `GET    /spaces/:id/channels`
- [ ] `POST   /spaces/:id/channels`
- [ ] `PATCH  /channels/:id`
- [ ] `DELETE /channels/:id`
- [ ] `PATCH  /spaces/:id/channels/reorder`

### Categories
- [ ] `POST   /spaces/:id/categories`
- [ ] `PATCH  /categories/:id`
- [ ] `DELETE /categories/:id`

### Overrides
- [ ] `GET    /channels/:id/overrides`
- [ ] `PUT    /channels/:id/overrides`
- [ ] `DELETE /channels/:id/overrides/:targetType/:targetId`
- [ ] `GET    /categories/:id/overrides`
- [ ] `PUT    /categories/:id/overrides`
- [ ] `DELETE /categories/:id/overrides/:tt/:tid`

### Messages
- [ ] `GET    /channels/:id/messages?before=&limit=`
- [ ] `POST   /channels/:id/messages`
- [ ] `PATCH  /messages/:id`
- [ ] `DELETE /messages/:id`

### DMs
- [ ] `GET    /dm`
- [ ] `POST   /dm`
- [ ] `POST   /dm/group`
- [ ] `PATCH  /dm/:id`
- [ ] `DELETE /dm/:id`
- [ ] `POST   /dm/:id/members`
- [ ] `DELETE /dm/:id/members`
- [ ] `DELETE /dm/:id/members/:targetUserId`
- [ ] `POST   /dm/:id/transfer`
- [ ] `POST   /dm/space-invite`
- [X] `GET    /dm/:id/messages?before=&limit=`
- [X] `POST   /dm/:id/messages`
- [ ] `PATCH  /dm/messages/:id`
- [ ] `DELETE /dm/messages/:id`

### Social
- [ ] `GET    /social/friends`
- [ ] `GET    /social/requests`
- [ ] `POST   /social/requests`
- [ ] `PATCH  /social/requests/:id`
- [ ] `DELETE /social/requests/:id`
- [ ] `DELETE /social/friends/:id`
- [ ] `GET    /social/discover?q=&limit=&offset=`
- [ ] `GET    /social/search?q=`

### Search
- [ ] `GET /channels/:id/search?q=&from=&has=&before=&after=&offset=&limit=`
- [ ] `GET /channels/:id/messages/around?messageId=&limit=`
- [ ] `GET /dm/:id/search?q=&from=&has=&before=&after=&offset=&limit=`
- [ ] `GET /dm/:id/messages/around?messageId=&limit=`

### Explore
- [ ] `GET    /spaces/explore?q=&limit=&offset=`
- [ ] `POST   /spaces/:id/public-join`
- [ ] `POST   /spaces/:id/request-join`
- [ ] `GET    /spaces/:id/join-requests?status=`
- [ ] `PATCH  /spaces/:id/join-requests/:rid`

### Directory
- [ ] `GET /directory/spaces`
- [ ] `GET /directory?q=&limit=&offset=`

### Uploads
- [ ] `POST   /api/files/`
- [ ] `HEAD   /api/files/:uploadId`
- [ ] `PATCH  /api/files/:uploadId`
- [ ] `DELETE /api/files/:uploadId`
- [ ] `GET    /uploads/:filename`

### GIF
- [ ] `GET /gif/enabled`
- [ ] `GET /gif/trending?limit=&pos=`
- [ ] `GET /gif/search?q=&limit=&pos=`

### Voice
- [ ] `POST /livekit/token`

### Instance
- [ ] `GET /instance/info`

### Settings
- [ ] `GET   /settings/streaming`
- [ ] `PATCH /settings/streaming`
- [ ] `GET   /settings/instance`
- [ ] `PATCH /settings/instance`

### Admin — Storage
- [ ] `GET  /admin/storage/stats`
- [ ] `GET  /admin/storage/orphans`
- [ ] `POST /admin/storage/cleanup`
- [ ] `POST /admin/storage/cleanup-media`
- [ ] `POST /admin/storage/cleanup-tus`

### Admin — Users
- [ ] `GET    /admin/users`
- [ ] `GET    /admin/users/instances`
- [ ] `PATCH  /admin/users/:id/role`
- [ ] `POST   /admin/users/:id/reset-password`
- [ ] `DELETE /admin/users/:id`

### Admin — Updates & Telemetry
- [ ] `GET /admin/instance/update-status`
- [ ] `GET /admin/telemetry`
- [ ] `PUT /admin/telemetry`
- [ ] `GET /admin/telemetry/preview`

### Admin — Invites
- [ ] `POST   /admin/invites`
- [ ] `GET    /admin/invites?status=`
- [ ] `PATCH  /admin/invites/:id`
- [ ] `POST   /admin/invites/:id/revoke`
- [ ] `POST   /admin/invites/:id/reinstate`
- [ ] `DELETE /admin/invites/:id`
- [ ] `GET    /admin/invites/:id/redemptions`

### Federation — Admin
- [ ] `POST   /federation/peer/initiate`
- [ ] `POST   /federation/peer/ensure`
- [ ] `GET    /federation/peers`
- [ ] `PATCH  /federation/peers/:id`
- [ ] `DELETE /federation/peers/:id`
- [ ] `DELETE /federation/peers/:id/permanent`
- [ ] `POST   /federation/peers/:id/reset`
- [ ] `POST   /federation/peers/:id/rotate`
- [ ] `POST   /federation/peers/:id/recheck`
- [ ] `GET    /federation/approval-requests`
- [ ] `POST   /federation/approval-requests/:id/approve`
- [ ] `POST   /federation/approval-requests/:id/deny`
- [ ] `GET    /federation/peering-subscriptions`
- [ ] `DELETE /federation/peering-subscriptions/:id`
- [ ] `GET    /federation/peering-notifications`
- [ ] `POST   /federation/peering-notifications/:id/read`
- [ ] `POST   /federation/peering-notifications/read-all`
- [ ] `GET    /federation/reset-events`
- [ ] `POST   /federation/reset-events/acknowledge`

### Utilities
- [ ] `GET  /utils/metadata?url=`
- [ ] `GET  /health`
- [ ] `POST /csp-report`

## WebSocket — Client → Server

### System
- [ ] `auth` — `{ token, client? }`
- [ ] `ping` — `{}`

### Messages
- [ ] `message_create` — `{ channelId, content, replyToId? }`
- [ ] `message_edit` — `{ messageId, content }`
- [ ] `message_delete` — `{ messageId }`
- [ ] `typing_start` — `{ channelId }`

### DM Messages
- [ ] `dm_message_create` — `{ dmChannelId, content?, attachments?, replyToId? }`
- [ ] `dm_message_edit` — `{ messageId, content }`
- [ ] `dm_message_delete` — `{ messageId }`
- [ ] `dm_typing_start` — `{ dmChannelId }`

### Reactions
- [ ] `reaction_add` — `{ messageId, emoji }`
- [ ] `reaction_remove` — `{ messageId, emoji }`

### Read State
- [X] `channel_ack` — `{ channelId, messageId }`
- [ ] `mark_unread` — `{ channelId, messageId }`

### Presence & Activity
- [ ] `presence_update` — `{ status }`
- [ ] `activity_update` — `{ activities[] }`

### Voice
- [ ] `voice_join` — `{ channelId }`
- [ ] `voice_leave` — `{}`
- [ ] `voice_status` — `{ isMuted, isDeafened, isCameraOn, isScreenSharing }`
- [ ] `voice_space_mute` — `{ userId, muted }`
- [ ] `voice_space_deafen` — `{ userId, deafened }`
- [ ] `voice_move` — `{ userId, targetChannelId }`
- [ ] `voice_disconnect` — `{ userId }`

### DM Calls
- [ ] `dm_call_start` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_accept` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_reject` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_end` — `{ dmChannelId?, federatedCallId? }`

## WebSocket — Server → Client

### System
- [X] `ready`
- [ ] `pong` — `{}`
- [ ] `error` — `{ message, code? }`

### Messages
- [ ] `message_created` — `{ message }`
- [ ] `message_updated` — `{ message }`
- [ ] `message_deleted` — `{ messageId, channelId }`
- [ ] `typing` — `{ channelId, userId, username }`
- [ ] `reaction_added` — `{ messageId, reaction }`
- [ ] `reaction_removed` — `{ messageId, userId, emoji }`
- [ ] `embeds_resolved` — `{ messageId, channelId, embeds[] }`

### DM Messages
- [ ] `dm_message_created` — `{ message }`
- [ ] `dm_message_updated` — `{ message }`
- [ ] `dm_message_deleted` — `{ messageId, dmChannelId }`
- [ ] `dm_typing` — `{ dmChannelId, userId, username }`
- [ ] `dm_typing_stop` — `{ dmChannelId, userId }`
- [ ] `dm_embeds_resolved` — `{ messageId, dmChannelId, embeds[] }`

### Presence
- [ ] `presence_update` — `{ userId, status, activities?, homeUserId?, homeInstance? }`
- [X] `user_updated` — `{ user }`

### Space / Channel Management
- [ ] `space_updated` — `{ space }`
- [ ] `member_joined` — `{ spaceId, member }`
- [ ] `member_left` — `{ spaceId, userId }`
- [ ] `member_banned` — `{ spaceId, reason }`
- [ ] `channel_created` — `{ channel, spaceId }`
- [ ] `channel_updated` — `{ channel, spaceId }`
- [ ] `channel_deleted` — `{ channelId, spaceId }`
- [ ] `category_created` — `{ category, spaceId }`
- [ ] `category_updated` — `{ category, spaceId }`
- [ ] `category_deleted` — `{ categoryId, spaceId }`
- [ ] `channel_layout_updated` — `{ spaceId, channels[], categories[] }`
- [ ] `space_layout_updated` — `{ layout[], folders[], updatedAt? }`

### DM Channel Management
- [X] `dm_channel_created` — `{ dmChannel }`
- [X] `dm_channel_closed` — `{ dmChannelId }`
- [ ] `dm_member_added` — `{ dmChannelId, user }`
- [ ] `dm_member_removed` — `{ dmChannelId, userId }`
- [ ] `dm_owner_updated` — `{ dmChannelId, newOwnerId, newOwnerHomeUserId?, newOwnerHomeInstance? }`

### Voice
- [ ] `voice_state_update` — `{ channelId, userId, action, channelElapsedSeconds? }`
- [ ] `voice_status_update` — `{ userId, channelId, isMuted, isDeafened, isCameraOn, isScreenSharing }`
- [ ] `space_voice_state` — `{ spaceId, voiceStates, ... }`
- [ ] `voice_space_muted` — `{ userId, channelId, spaceId, muted }`
- [ ] `voice_space_deafened` — `{ userId, channelId, spaceId, deafened }`
- [ ] `voice_permission_muted` — `{ userId, spaceId, muted }`
- [ ] `voice_moved` — `{ userId, oldChannelId, newChannelId }`
- [ ] `voice_disconnected` — `{ userId, channelId, reason? }`

### DM Calls
- [ ] `dm_call_incoming` — `{ dmChannelId?, federatedCallId, callerId, callerName, ... }`
- [ ] `dm_call_accepted` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_rejected` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_ended` — `{ dmChannelId?, federatedCallId? }`
- [ ] `dm_call_undeliverable` — `{ phase, failures[], terminal }`

### Social
- [ ] `friend_request_received` — `{ request }`
- [ ] `friend_request_accepted` — `{ friend, requestId }`
- [ ] `friend_request_declined` — `{ requestId, userId }`
- [ ] `friend_request_cancelled` — `{ requestId, userId }`
- [ ] `friend_removed` — `{ userId }`

### Discovery
- [ ] `join_request_received` — `{ request }`
- [ ] `join_request_accepted` — `{ request, space }`
- [ ] `join_request_declined` — `{ request }`

### Federation
- [ ] `federation_file_rejected` — `{ messageId, dmChannelId, attachmentId, affectedUsers[] }`
- [ ] `federation_approval_request_received` — `{}`
- [ ] `federation_peer_reset_detected` — `{ origin }`
- [ ] `peering_subscription_changed` — `{}`
- [ ] `peering_notification_received` — `{ kind }`
