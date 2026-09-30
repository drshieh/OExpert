REM Initialize Git
git init
git branch -M main

REM Stage everything
git add .

REM Review what will be committed
git status

REM If it looks right, commit
git config --global user.email "drshieh@drssw.com" user.password "Cathy92$$88"

git commit -m "Initial public release of OExpert"

REM Create the repo on GitHub (via web interface or gh CLI)
REM Then:
git remote add origin https://github.com/drshieh/OExpert.git
git push -u origin main