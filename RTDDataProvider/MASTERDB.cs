using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider
{
    /// <summary>
    /// MDB对应的枚举
    /// </summary>
    public enum MASTERDB
    {
        UNIT_MASTER = 10,
        PARTY_SKILL_MASTER = 11,
        PARTY_SKILL_RANK_MASTER = 12,
        ACTIVE_SKILL_MASTER = 13,
        ACTIVE_SKILL_RANK_MASTER = 14,
        UNIT_TALK_MASTER = 15,  //not exist
        PANEL_SKILL_MASTER = 16,
        PANEL_SKILL_RANK_MASTER = 17,
        ACCESSORY_MASTER = 18,
        //MAX = 19,   //not exist
        ENEMY_UNIT_MASTER = 20,
        ENEMY_TABLE_MASTER,  //not exist
        ENEMY_DROP_MASTER = 22,  //not exist
        QUEST_MASTER = 30,
        QUEST_CATEGORY_MASTER = 31,
        QUEST_CHALLENGE_MASTER = 32,
        QUEST_CHALLENGE_REWARD_MASTER = 33,
        SP_EVENT_MASTER = 34,
        GACHA_ITEM_MASTER = 40,  //not exist
        GACHA_TABLE_MASTER = 41,  //not exist
        SHOP_PRODUCT_MASTER = 42,   //exist in iOS only
        SHOP_PRODUCT_MASTER_ANDROID = 43,
        LOGIN_BONUS_MASTER = 51,
        SEQUENCE_LOGIN_BONUS_MASTER = 52,
        USER_RANK_MASTER = 60,
        LEVELDATA_LIST_MASTER = 70,  //not exist
        GLOBAL_PARAM_MASTER = 90,
    }
}