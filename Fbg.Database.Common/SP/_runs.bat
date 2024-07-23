@echo off 

echo ************** RUNNING THE FOLLWING SCRIPTS ****************
for %%x in (*.sql) do echo %%x 
echo ************** RUNNING THE FOLLWING SCRIPTS ****************

pause

for %%y in (FbgCommon) do (

	echo ************ RUNNING  ON  %%y  ********************
 	for %%x in (*.sql) do ( 
		echo ** %%x 
		sqlcmd -S localhost -d %%y -i %%x  -r1 1> NUL
	)

) 

pause