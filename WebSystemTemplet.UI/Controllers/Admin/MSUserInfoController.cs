﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSystemTemplet.BLL.Admin;
using WebSystemTemplet.Model;
using WebSystemTemplet.UI.Filters;
using WebSystemTemplet.Utility;

namespace WebSystemTemplet.UI.Controllers.Admin
{
    public class MSUserInfoController : PCBaseController
    {
        [HttpGet]
        public ActionResult GetAllBaseUserInfoList(Models.Admin.MSGetUserInfoListIn InModel)
        {
            SqlParams sqlParams = new SqlParams();
            sqlParams.PageIndex = 9999;
            sqlParams.PageSize = 1;
            sqlParams.addUsefulParam("roleId", InModel.RoleId);
            sqlParams.addUsefulParam("majorId", InModel.MajorId);
            sqlParams.addUsefulParam("classId", InModel.ClassId);
            sqlParams.addUsefulParam("keyWords", InModel.KeyWords);

            List<Model.Admin.MSUserInfo> userInfoList = BLL.Admin.MSUserInfoBll.GetAllBaseUserInfoList(sqlParams);

            return Json(new
            {
                Rows = userInfoList.Select(u => new
                {
                    u.UserID,
                    u.UserName,
                    u.RealName,
                })
            }, JsonRequestBehavior.AllowGet);
        }

        #region 教职工管理

        [ActionLogFilter(InitLogMsg = "访问教职工列表页面")]
        public ActionResult TeacherList()
        {
            // 获取所有的专业
            List<Model.Admin.MSDepartmentInfo> departmentList = BLL.Admin.MSDepartmentInfoBll.GetAllDepartmentInfoByLevel((int)Model.DepartmentLevel.专业);
            ViewBag.DepartmentList = departmentList;
            return View("~/Views/Admin/MSUserInfo/TeacherList.cshtml");
        }

        [HttpGet]
        public ActionResult GetTeacherList(Models.Admin.MSGetUserInfoListIn InModel)
        {
            SqlParams sqlParams = new SqlParams();
            sqlParams.PageIndex = Converter.TryToInt32(InModel.PageIndex, 1);
            sqlParams.PageSize = Converter.TryToInt32(InModel.PageSize, sqlParams.PageSize);
            sqlParams.addUsefulParam("roleId", (int)RoleType.教职工);
            sqlParams.addUsefulParam("PositionType", InModel.PositionType);
            sqlParams.addUsefulParam("majorId", InModel.MajorId);
            sqlParams.addUsefulParam("keyWords", InModel.KeyWords);

            int allCount = 0;
            List<Model.Admin.MSUserInfo> userInfoList = BLL.Admin.MSUserInfoBll.GetAllUserInfoList(sqlParams, out allCount);

            return Json(new
            {
                Rows = userInfoList.Select(u => new
                {
                    u.UserID,
                    u.UserName,
                    u.RealName,
                    Gender = u.Gender == 0 ? "女" : "男",
                    MajorName = MSDepartmentInfoBll.GetDepartmentNameById(u.MajorID).IfEmptyToString("--"),
                    u.PositionName,
                }),
                AllCount = allCount
            }, JsonRequestBehavior.AllowGet);
        }

        [ActionLogFilter(InitLogMsg = "访问添加教职工信息页面")]
        public ActionResult TeacherAddPage()
        {
            // 获取所有的专业
            List<Model.Admin.MSDepartmentInfo> departmentList = BLL.Admin.MSDepartmentInfoBll.GetAllDepartmentInfoByLevel((int)Model.DepartmentLevel.专业);
            ViewBag.DepartmentList = departmentList;

            return View("~/Views/Admin/MSUserInfo/TeacherAdd.cshtml");
        }

        [HttpPost]
        [ActionLogFilter(InitLogMsg = "添加教职工信息")]
        public ActionResult AddTeacherCallBack(Models.Admin.MSUserInfoModel InModel)
        {
            string msg = "OK";

            // 添加用户
            Model.Admin.MSUserInfo userInfo = new Model.Admin.MSUserInfo()
            {
                UserName = InModel.UserName,
                Password = Security.getMD5ByStr(InModel.Password),
                RealName = InModel.RealName,
                RoleID = (int)Model.RoleType.教职工,
                SchoolID = 1, //默认学院
                MajorID = Converter.TryToInt64(InModel.MajorId),
                ClassID = 0,
                Gender = InModel.Gender,
                Telephone = InModel.Telephone,
                QQ = InModel.QQ,
                Email = InModel.Email,
                Remark = InModel.Remark,
                CreateTime = DateTime.Now,
                CreateUser = Identity.LoginUserInfo.UserID,
            };
            userInfo.UserID = BLL.Admin.MSUserInfoBll.AddUserInfo(userInfo);
            if (userInfo.UserID > 0)
            {
                // 添加默认岗位
                Model.Admin.MSUserPositionRelation relation = new Model.Admin.MSUserPositionRelation()
                {
                    UserID = userInfo.UserID,
                    CreateTime = DateTime.Now,
                    CreateUser = Identity.LoginUserInfo.UserID,
                };
                if (InModel.IsPresident == "1")
                {
                    // 院长
                    relation.PositionID = userInfo.SchoolID;//院长岗位ID
                }
                else
                {
                    // 讲师
                    relation.PositionID = MSPositionInfoBll.GetPositionIdByDepartmentIdAndPositionType(userInfo.MajorID, (int)Model.PositionType.讲师);
                }
                MSUserPositionRelationBll.AddUserPositionRelation(relation);
            }
            else
            {
                msg = "添加失败，请重试";
            }
            return Json(new { Message = msg });
        }

        [ActionLogFilter(InitLogMsg = "访问编辑教职工信息页面")]
        public ActionResult TeacherEditPage(long userId)
        {
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo != null)
            {
                // 完善信息
                userInfo.SchoolName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.SchoolID);
                userInfo.MajorName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.MajorID);
            }
            return View("~/Views/Admin/MSUserInfo/TeacherEdit.cshtml", userInfo);
        }

        [ActionLogFilter(InitLogMsg = "查看教职工信息详情")]
        public ActionResult GetTeacherDetails(long userId)
        {
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo != null)
            {
                // 完善信息
                userInfo.SchoolName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.SchoolID);
                userInfo.MajorName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.MajorID);
                // 岗位信息

                return Json(new { Message = "OK", UserInfo = userInfo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "未找到该教师资料，请刷新列表重新操作！" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 学生管理

        [ActionLogFilter(InitLogMsg = "访问学生列表页面")]
        public ActionResult StudentList()
        {
            // 获取所有的专业
            List<Model.Admin.MSDepartmentInfo> majorList = BLL.Admin.MSDepartmentInfoBll.GetAllDepartmentInfoByLevel((int)Model.DepartmentLevel.专业);
            ViewBag.MajorList = majorList;

            return View("~/Views/Admin/MSUserInfo/StudentList.cshtml");
        }

        [HttpGet]
        public ActionResult GetStudentList(Models.Admin.MSGetUserInfoListIn InModel)
        {
            SqlParams sqlParams = new SqlParams();
            sqlParams.PageIndex = Converter.TryToInt32(InModel.PageIndex, 1);
            sqlParams.PageSize = Converter.TryToInt32(InModel.PageSize, sqlParams.PageSize);
            sqlParams.addUsefulParam("roleId", (int)RoleType.学生);
            sqlParams.addUsefulParam("PositionType", InModel.PositionType);
            sqlParams.addUsefulParam("majorId", InModel.MajorId);
            sqlParams.addUsefulParam("classId", InModel.ClassId);
            sqlParams.addUsefulParam("keyWords", InModel.KeyWords);

            int allCount = 0;
            List<Model.Admin.MSUserInfo> userInfoList = BLL.Admin.MSUserInfoBll.GetAllUserInfoList(sqlParams, out allCount);

            return Json(new
            {
                Rows = userInfoList.Select(u => new
                {
                    u.UserID,
                    u.UserName,
                    u.RealName,
                    Gender = u.Gender == 0 ? "女" : "男",
                    MajorName = MSDepartmentInfoBll.GetDepartmentNameById(u.MajorID).IfEmptyToString("--"),
                    ClassName = MSDepartmentInfoBll.GetDepartmentNameById(u.ClassID).IfEmptyToString("--"),
                    u.PositionName,
                }),
                AllCount = allCount
            }, JsonRequestBehavior.AllowGet);
        }

        [ActionLogFilter(InitLogMsg = "访问添加学生信息页面")]
        public ActionResult StudentAddPage()
        {
            // 获取所有的专业
            List<Model.Admin.MSDepartmentInfo> majorList = BLL.Admin.MSDepartmentInfoBll.GetAllDepartmentInfoByLevel((int)Model.DepartmentLevel.专业);
            ViewBag.MajorList = majorList;

            return View("~/Views/Admin/MSUserInfo/StudentAdd.cshtml");
        }

        [ActionLogFilter(InitLogMsg = "添加学生信息")]
        [HttpPost]
        public ActionResult AddStudentCallBack(Models.Admin.MSUserInfoModel InModel)
        {
            string msg = "OK";

            // 添加用户
            Model.Admin.MSUserInfo userInfo = new Model.Admin.MSUserInfo()
            {
                UserName = InModel.UserName,
                Password = Security.getMD5ByStr(InModel.Password),
                RealName = InModel.RealName,
                RoleID = (int)Model.RoleType.学生,
                SchoolID = 1, //默认学院
                MajorID = Converter.TryToInt64(InModel.MajorId),
                ClassID = Converter.TryToInt64(InModel.ClassId),
                Gender = InModel.Gender,
                Telephone = InModel.Telephone,
                QQ = InModel.QQ,
                Email = InModel.Email,
                Remark = InModel.Remark,
                CreateTime = DateTime.Now,
                CreateUser = Identity.LoginUserInfo.UserID,
            };
            userInfo.UserID = BLL.Admin.MSUserInfoBll.AddUserInfo(userInfo);
            if (userInfo.UserID > 0)
            {
                // 添加默认岗位
                Model.Admin.MSUserPositionRelation relation = new Model.Admin.MSUserPositionRelation()
                {
                    UserID = userInfo.UserID,
                    PositionID = MSPositionInfoBll.GetPositionIdByDepartmentIdAndPositionType(userInfo.ClassID, (int)Model.PositionType.学生),
                    CreateTime = DateTime.Now,
                    CreateUser = Identity.LoginUserInfo.UserID,
                };

                MSUserPositionRelationBll.AddUserPositionRelation(relation);
            }
            else
            {
                msg = "添加失败，请重试";
            }
            return Json(new { Message = msg });
        }


        [ActionLogFilter(InitLogMsg = "访问编辑学生信息页面")]
        public ActionResult StudentEditPage(long userId)
        {
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo != null)
            {
                // 完善信息
                userInfo.SchoolName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.SchoolID);
                userInfo.MajorName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.MajorID);
                userInfo.ClassName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.ClassID);
            }
            return View("~/Views/Admin/MSUserInfo/StudentEdit.cshtml", userInfo);
        }

        [ActionLogFilter(InitLogMsg = "查看学生信息详情")]
        public ActionResult GetStudentDetails(long userId)
        {
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo != null)
            {
                // 完善信息
                userInfo.SchoolName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.SchoolID);
                userInfo.MajorName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.MajorID);
                userInfo.ClassName = BLL.Admin.MSDepartmentInfoBll.GetDepartmentNameById(userInfo.ClassID);
                // 岗位信息

                return Json(new { Message = "OK", UserInfo = userInfo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "未找到该学生资料，请刷新列表重新操作！" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        [ActionLogFilter(InitLogMsg = "修改用户资料")]
        [HttpPost]
        public ActionResult EditUserInfoCallBack(Models.Admin.MSUserInfoModel InModel)
        {
            string msg = "OK";
            long userId = Converter.TryToInt64(InModel.UserId);
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo == null)
            {
                return Json(new { Message = "资料不存在，修改失败！" });
            }

            // 修改用户信息
            userInfo.RealName = InModel.RealName;
            userInfo.Gender = InModel.Gender;
            userInfo.Telephone = InModel.Telephone;
            userInfo.QQ = InModel.QQ;
            userInfo.Email = InModel.Email;
            userInfo.Remark = InModel.Remark;
            userInfo.UpdateTime = DateTime.Now;
            userInfo.UpdateUser = Identity.LoginUserInfo.UserID;

            if (BLL.Admin.MSUserInfoBll.EditUserInfo(userInfo))
            {
                msg = "OK";
            }
            else
            {
                msg = "修改失败，请重试";
            }
            return Json(new { Message = msg });
        }


        [ActionLogFilter(InitLogMsg = "删除用户资料")]
        [HttpPost]
        public ActionResult DeleteUser(long userId)
        {
            // 查询用户信息
            Model.Admin.MSUserInfo userInfo = BLL.Admin.MSUserInfoBll.GetSingleUserInfo(userId);
            if (userInfo == null)
            {
                return Json(new { Message = "该用户不存在或已被删除！！" });
            }
            if (BLL.Admin.MSUserInfoBll.DeleteSingleUserInfo(userId))
            {
                return Json(new { Message = "OK" });
            }
            else
            {
                return Json(new { Message = "删除失败！" });
            }

        }

        [HttpGet]
        public ActionResult CheckNameUseful(string userName)
        {
            if (!userName.IsNullOrWhiteSpace())
            {
                userName = userName.Trim();
                bool re = BLL.Admin.MSUserInfoBll.IsExistUserName(userName);
                return Json(new { Message = re ? "Error" : "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "用户名不能为空！" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}