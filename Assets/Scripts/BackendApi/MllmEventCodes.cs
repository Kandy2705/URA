public static class MllmEventCodes
{
    // Level 1
    public const string Lvl1MapIntro = "lvl1_map_intro";
    public const string Lvl1RulesExplanation = "lvl1_rules_explanation";
    public const string Lvl1ReadShoppingList = "lvl1_read_shopping_list";
    public const string VoiceChatTrigger = "voice_chat_trigger";

    // Level 2
    public const string Lvl2MapIntro = "lvl2_map_intro";
    public const string Lvl2RulesExplanation = "lvl2_rules_explanation";
    public const string Lvl2ReadShoppingList = "lvl2_read_shopping_list";
    public const string Lvl2PrioritySetup = "lvl2_priority_setup";
    public const string Lvl2HiddenTaskSetup = "lvl2_hidden_task_setup";
    public const string Lvl2OutOfStockWarning = "lvl2_out_of_stock_warning";
    public const string Lvl2ShiftingOrder = "lvl2_shifting_order";
    public const string Lvl2FlashSaleDistraction = "lvl2_flash_sale_distraction";
    public const string Lvl2NpcDistraction = "lvl2_npc_distraction";
    public const string Lvl2TimeUpAnnouncement = "lvl2_time_up_announcement";
    public const string Lvl2CheckoutCheck = "lvl2_checkout_check";
}

public static class MllmGamePhases
{
    public const string PreGame = "PRE_GAME";
    public const string InGame = "IN_GAME";
    public const string PostGame = "POST_GAME";
}

public static class MllmActionCodes
{
    public const string AnimGreet = "Anim_Greet";
    public const string AnimPointForward = "Anim_PointForward";
    public const string AnimPointUI = "Anim_PointUI";
    public const string AnimExplain = "Anim_Explain";
    public const string AnimAlert = "Anim_Alert";
    public const string AnimShakeHead = "Anim_ShakeHead";
    public const string AnimNpcOffer = "Anim_NPCOffer";
    public const string AudioPhoneCall = "Audio_PhoneCall";
    public const string AudioBroadcast = "Audio_Broadcast";
}