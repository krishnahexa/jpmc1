package $PACKAGE_NAME$;

import org.hibernate.dialect.identity.AbstractTransactSQLIdentityColumnSupport;

public class SQLServerIdentityColumnSupport extends AbstractTransactSQLIdentityColumnSupport {

	@Override
	public String appendIdentitySelectToInsert(String insertSQL) {
		return insertSQL + " select @@identity";
	}
}
