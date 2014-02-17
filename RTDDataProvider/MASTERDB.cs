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
        USER_RANK_MASTER = 60,
        UNIT_MASTER = 10,
        PARTY_SKILL_MASTER,
        PARTY_SKILL_RANK_MASTER,
        ACTIVE_SKILL_MASTER,
        ACTIVE_SKILL_RANK_MASTER,
        ENEMY_UNIT_MASTER = 20,
        ENEMY_TABLE_MASTER,
        ENEMY_DROP_MASTER,  //not exist
        QUEST_MASTER = 30,
        QUEST_CATEGORY_MASTER,
        GACHA_ITEM_MASTER = 40, //not exist
        GACHA_TABLE_MASTER, //not exist
        SHOP_PRODUCT_MASTER,    //not exist
        SHOP_PRODUCT_MASTER_ANDROID,
        LOGIN_BONUS_MASTER = 51,
        SEQUENCE_LOGIN_BONUS_MASTER,
        LEVELDATA_LIST_MASTER = 70, //not exist
        UNIT_TALK_MASTER = 15,
        GLOBAL_PARAM_MASTER = 90,
        QUEST_CHALLENGE_MASTER = 32,
        QUEST_CHALLENGE_REWARD_MASTER,
        SP_EVENT_MASTER,
        PANEL_SKILL_MASTER = 16,
        PANEL_SKILL_RANK_MASTER,
        MAX = 22    //not exist
    }
}
