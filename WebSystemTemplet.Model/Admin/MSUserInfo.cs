﻿using System;
using WebSystemTemplet.Utility;

namespace WebSystemTemplet.Model.Admin
{
    /// <summary>
    /// MSUserInfo
    /// </summary>
    public class MSUserInfo
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleID { get; set; }

        /// <summary>
        /// 学院ID
        /// </summary>
        public long SchoolID { get; set; }

        /// <summary>
        /// 专业ID
        /// </summary>
        public long MajorID { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassID { get; set; }

        /// <summary>
        /// 性别：0-女 1-男
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 头像相对路径
        /// </summary>
        public string IconUrl
        {
            get
            {
                string Suffix = new Random(Converter.TryToInt32(UserID) * 100).Next(1, 6).ToString();// 1-5 随机
                return "/Resources/adminassets/img/head/{0}{1}.gif".Format(Gender, Suffix);
            }
            set { }
        }

        /// <summary>
        /// QQ号
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public long UpdateUser { get; set; }

        /// <summary>
        /// 是否删除 1 已删除 ;0 未删除
        /// </summary>
        public byte Deleted { get; set; }


        // 扩展字段//////////////////////////////////////////////////////

        /// <summary>
        /// 职位名称集合
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// 学院名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 专业名称
        /// </summary>
        public string MajorName { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 上次登录IP
        /// </summary>
        public string LastIpAddress { get; set; }
    }
}



