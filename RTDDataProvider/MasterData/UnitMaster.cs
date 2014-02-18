using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class UnitMaster
    {
        public int id;
        public int g_id;
        public int ui_id;
        public int flag_no;
        public string name;
        public string model;
        public int shadow;
        public int category;
        public int attribute;
        public int sub_a1;
        public int style;
        public int sub_c1;
        public int sub_c2;
        public int sub_c3;
        public int sub_c4;
        public int kind;
        public int lv_max;
        public int rev_unit_id;
        public int ultimate_rev_unit_id_fire;
        public int ultimate_rev_unit_id_water;
        public int ultimate_rev_unit_id_shine;
        public int ultimate_rev_unit_id_dark;
        public int max_attribute_exp;
        public int max_attribute_exp_fire;
        public int max_attribute_exp_water;
        public int max_attribute_exp_shine;
        public int max_attribute_exp_dark;
        public int need_pt;
        public int set_pt;
        public int grow;
        public int mix;
        public int cost;
        public int sale;
        public int ap_rec_val;
        public int bonus_limit_base;
        public int yorisiro;
        public int present;
        public int material_type;
        public int life;
        public int attack;
        public int heal;
        public int p_skill_id;
        public int a_skill_id;
        public int panel_skill_id;
        public string story;
        public string ct_text;
        public string sct_text;
        public string sct6_text;
        public string a_skill_text;
        public string icon_name;
        //public UIAtlas cutin_atlas;
        public string cutin_name1;
        public string cutin_name2;
    }
}
