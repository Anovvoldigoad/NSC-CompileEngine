#!/bin/bash
# push.sh — jalankan dari dalam folder NSC_ModManager_CLI
# Usage: bash push.sh "pesan commit"

MSG="${1:-update}"

git add -A
git commit -m "$MSG"
git push

echo ""
echo "✅ Push selesai. Cek Actions di:"
REMOTE=$(git remote get-url origin 2>/dev/null | sed 's/git@github.com:/https:\/\/github.com\//;s/\.git$//')
echo "  ${REMOTE}/actions"
