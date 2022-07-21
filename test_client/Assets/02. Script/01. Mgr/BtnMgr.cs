using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class BtnMgr : Singleton<BtnMgr>
    {
        #region Login

        public void OnEnterJoinBtn()
        {
            // TryGetValue
            // - Key값에 해당하는 value를 가져온다.
            StageMgr.Instance.ChangeState(StageMgr.STAGE.LOGIN, StageMgr.STAGE.JOIN);

            Debug.Log("가입화면 들어가기");
        }

        #endregion

        #region Join

        public void OnEnterLoginBtn()
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.JOIN, StageMgr.STAGE.LOGIN);

            Debug.Log("로그인 버튼 클릭");
        }

        #endregion

        #region Menu

        public void OnEnterMainMenuBtn() // mainmenu 입장
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.MENU, StageMgr.STAGE.MAIN_MENU);
            Debug.Log("Enter_MainMenu");
        }

        #endregion

        #region Main Menu



        #endregion

        #region Lobby

        #endregion

    }
}

