@echo off 

echo ************** RUNNING THE FOLLWING SCRIPTS ****************
for %%x in (*.sql) do echo %%x 
echo ************** RUNNING THE FOLLWING SCRIPTS ****************



for %%y in (fbg1) do (

	echo ************ RUNNING  ON  %%y  ********************
 	for %%x in (*.sql) do ( 
		echo ** %%x 
		sqlcmd -S localhost -d %%y -i %%x -E -r1 1> NUL
	)

) 
