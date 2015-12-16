@echo off
git remote set-url origin git://opensimulator.org/git/opensim
git pull origin master
git remote set-url origin https://github.com/simpleSim/opensim.git
git push origin master