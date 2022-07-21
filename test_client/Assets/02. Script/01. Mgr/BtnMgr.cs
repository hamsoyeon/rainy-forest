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
            // - Key���� �ش��ϴ� value�� �����´�.
            StageMgr.Instance.ChangeState(StageMgr.STAGE.LOGIN, StageMgr.STAGE.JOIN);

            Debug.Log("����ȭ�� ����");
        }

        #endregion

        #region Join

        public void OnEnterLoginBtn()
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.JOIN, StageMgr.STAGE.LOGIN);

            Debug.Log("�α��� ��ư Ŭ��");
        }

        #endregion

        #region Menu

        public void OnEnterMainMenuBtn() // mainmenu ����
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

