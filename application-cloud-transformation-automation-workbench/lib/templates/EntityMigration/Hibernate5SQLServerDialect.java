package $PACKAGE_NAME$;

import org.hibernate.dialect.SQLServerDialect;
import org.hibernate.dialect.identity.IdentityColumnSupport;



public class Hibernate5SQLServerDialect extends SQLServerDialect {

    public String appendIdentitySelectToInsert(String insertSQL) {
        return insertSQL + " select @@identity";
    }

    @Override
    public IdentityColumnSupport getIdentityColumnSupport() {
        return new SQLServerIdentityColumnSupport();
    }
}
