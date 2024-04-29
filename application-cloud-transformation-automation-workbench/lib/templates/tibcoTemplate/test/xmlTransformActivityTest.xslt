<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:math="http://www.oracle.com/XSL/Transform/java/java.lang.Math"
	xmlns:tib="http://www.oracle.com/XSL/Transform/java/amazebasePackage.utils.TibcoLibUtil"
	exclude-result-prefixes="math tib">

	<xsl:template match="/">
		<AIMReply>
			<result>
				<xsl:value-of select="&quot;failure&quot;" />
			</result>
			<errorCode>
				<xsl:value-of select="&quot;AIMAC99901&quot;" />
			</errorCode>
			<errorMessage>
				<xsl:value-of
					select="&quot;AIM Application Outage&quot;" />
			</errorMessage>
		</AIMReply>
	</xsl:template>
</xsl:stylesheet>