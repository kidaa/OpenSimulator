@echo off
git remote add origin git://opensimulator.org/git/opensim
git pull origin master
git remote add origin https://github.com/simpleSim/opensim.git
git push origin master