@echo off
1) git remote set-url origin git://opensimulator.org/git/opensim
      (hier Merge conflict bearbeiten)
2) git pull origin master
3) git remote set-url origin https://github.com/ChrisWeymann/OpenSimulator.git
3) git push origin master


fix Merge conflict:
1)  Die Daten öffnen und so machen wie es sein soll
2)  Für jede Datei: git add <file>
3)  git commit
4)  bei punkt 2 weiter machen
