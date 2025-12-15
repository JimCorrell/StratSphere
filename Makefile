.PHONY: dev api web

# Start API and Next.js dev server together. Ctrl+C stops both.
dev:
	@echo "Launching StratSphere API and web..."
	@trap 'kill 0' INT TERM EXIT; \
	  (cd src/StratSphere.Api && ASPNETCORE_ENVIRONMENT=Development dotnet watch run --no-hot-reload) & \
	  (cd web && npm run dev) & \
	  wait

# API only
api:
	@cd src/StratSphere.Api && ASPNETCORE_ENVIRONMENT=Development dotnet watch run --no-hot-reload

# Web only
web:
	@cd web && npm run dev
