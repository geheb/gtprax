#!/bin/sh

DAY="$(date +'%d')"
BACKUPFILE="gtprax-app-$DAY.7z"
BACKUPDIR="/opt/gtprax-backup"

echo "remove old file"
rm -f "$BACKUPDIR/$BACKUPFILE" || true

echo "create backup"
7z a -mhe=on "$BACKUPDIR/$BACKUPFILE" /opt/gtprax/ -p"***" > /dev/null 2>&1

echo "sync backup"
rclone sync --exclude gtprax-backup.sh $BACKUPDIR remote:gtprax-app
