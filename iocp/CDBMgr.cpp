#include "pch.h"
#include "CDBMgr.h"
#include "CLoginMgr.h"
#include "CLock.h"
#include "CLockGuard.h"

CDBMgr* CDBMgr::m_instance = nullptr;

CDBMgr* CDBMgr::GetInst()
{
	return m_instance;
}

void CDBMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CDBMgr();
}

void CDBMgr::Destroy()
{
	delete m_instance;
}

CDBMgr::CDBMgr()
	: m_lock(new CLock)
{
}

CDBMgr::~CDBMgr()
{
	delete m_lock;
}

void CDBMgr::Init()
{
	mysql_init(&m_mysql);
	//mysql_options(&m_mysql, MYSQL_SET_CHARSET_NAME, "utf8");
	//mysql_options(&m_mysql, MYSQL_INIT_COMMAND, "SET NAMES utf8");
	if (!mysql_real_connect(&m_mysql, HOST_IP, USER, PASSWORD, DATABASE, 3306, NULL, 0))
	{
		std::cout << "mysql connected error : " << mysql_error(&m_mysql) << std::endl;
		exit(-1);
	}
	mysql_query(&m_mysql, "set names euckr;");
	m_stmt_set = mysql_stmt_init(&m_mysql);

	CLoginMgr::GetInst()->SetJoinlist(GetJoin());
}

void CDBMgr::End()
{
	mysql_stmt_close(m_stmt_set);
	mysql_close(&m_mysql);
}

void CDBMgr::SetJoin(list<t_UserInfo*> _users)
{
	char temp[100];
	ZeroMemory(temp, 100);
	//테이블 삭제 및 생성 쿼리문 실행 후
	sprintf(temp, "truncate table jointbl" /* table이름 */);
	if (mysql_query(&m_mysql, (char*)temp))
	{
		printf("** %s **\n", mysql_error(&m_mysql));
	}
	ZeroMemory(temp, 100);
	//db에 넣는다.
	for (t_UserInfo* user : _users)
	{
		sprintf(temp, "insert into jointbl values('%s','%s','%s');", user->id, user->pw, user->nickname);
		if (mysql_query(&m_mysql, (char*)temp))
		{
			printf("** %s **\n", mysql_error(&m_mysql));
		}
	}
}

list<t_UserInfo*> CDBMgr::GetJoin()
{
	const char* query;
	char temp[100];
	ZeroMemory(temp, 100);
	sprintf(temp, "select * from jointbl");
	query = temp;

	if (mysql_query(&m_mysql, (char*)query))
	{
		printf("** %s **\n", mysql_error(&m_mysql));
	}
	m_sql_result = mysql_store_result(&m_mysql);

	//mysql_set_character_set(&m_mysql, "utf8");
	list<t_UserInfo*>users;
	TCHAR ID[STRING_SIZE], PW[STRING_SIZE], NICK[STRING_SIZE];
	ZeroMemory(ID, STRING_SIZE * 2);
	ZeroMemory(PW, STRING_SIZE * 2);
	ZeroMemory(NICK, STRING_SIZE * 2);

	while ((m_sql_row = mysql_fetch_row(m_sql_result)) != NULL)
	{
		TCHAR id[BUFSIZE]; ZeroMemory(id, sizeof(id));
		TCHAR pw[BUFSIZE]; ZeroMemory(pw, sizeof(pw));
		TCHAR nick[BUFSIZE]; ZeroMemory(nick, sizeof(nick));

		// 멀티바이트를 유니코드로 변환한다.
		MultiByteToWideChar(CP_ACP, 0, m_sql_row[0], strlen(m_sql_row[0]), id, strlen(m_sql_row[0]));
		MultiByteToWideChar(CP_ACP, 0, m_sql_row[1], strlen(m_sql_row[1]), pw, strlen(m_sql_row[1]));
		MultiByteToWideChar(CP_ACP, 0, m_sql_row[2], strlen(m_sql_row[2]), nick, strlen(m_sql_row[2]));

		// 유니코드로 변환된 멀티바이트를 복사시킨다.
		memcpy(ID, id, strlen(m_sql_row[0]) * 2);
		memcpy(PW, pw, strlen(m_sql_row[1]) * 2);
		memcpy(NICK, nick, strlen(m_sql_row[2]) * 2);

		//memcpy(PW, m_sql_row[1], sizeof(m_sql_row[1]));
		//memcpy(NICK, m_sql_row[2], sizeof(m_sql_row[2]));
		t_UserInfo* user = new t_UserInfo(ID, PW, NICK);
		users.push_back(user);
	}

	//int rowcount = mysql_num_rows(m_sql_result);
	//ZeroMemory(m_sql_row, sizeof(MYSQL_ROW) * rowcount);

	mysql_free_result(m_sql_result);
	return users;
}

void CDBMgr::InsertJointbl(t_UserInfo* _user)
{
	CLock_Guard<CLock> lock(m_lock);

	char temp[100]; ZeroMemory(temp, 100);
	sprintf(temp, "insert into jointbl values('%s','%s','%s');", _user->id, _user->pw, _user->nickname);
	if (mysql_query(&m_mysql, (char*)temp))
	{
		printf("** %s **\n", mysql_error(&m_mysql));
	}
}

void CDBMgr::InsertJoinLog(TCHAR* _content)
{
	CLock_Guard<CLock> lock(m_lock);

	const char* query;
	char temp[BUFSIZE];
	ZeroMemory(temp, BUFSIZE);
	sprintf(temp, "insert into joinlogtbl values(null, curdate(), curtime(), '%s')", _content);
	query = temp;
	if (mysql_query(&m_mysql, (char*)query))
	{
		printf("** %s **\n", mysql_error(&m_mysql));
	}
}

