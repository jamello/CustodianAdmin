declare	@RPT_ST_DATE	DATETIME 
	,@RPT_END_DATE	DATETIME 
	,@RPT_ST_BRANCH	CHAR(5)
	,@RPT_END_BRANCH CHAR(5)
	
select @RPT_ST_DATE	= '20130101' 
	,@RPT_END_DATE	= '20131231' 
	,@RPT_ST_BRANCH = 'aa'
	,@RPT_END_BRANCH= 'zz'
	
	exec ABSSP_ADMIN_ELEC_REPORT @RPT_ST_DATE,@RPT_END_DATE,@RPT_ST_BRANCH,@RPT_END_BRANCH