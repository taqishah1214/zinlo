#!/bin/bash
docker-compose -f docker-compose-migration.yml build --no-cache
docker-compose -f docker-compose-migration.yml up 