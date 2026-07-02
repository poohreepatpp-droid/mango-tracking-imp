# ── Stage 1: Build ────────────────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .

# API_BASE_URL must be the URL that the **browser** can reach (public IP/domain)
ARG API_BASE_URL=http://localhost:5000
ENV API_BASE_URL=$API_BASE_URL

RUN npm run build

# ── Stage 2: Runtime ──────────────────────────────────────────────────────
FROM node:22-alpine
WORKDIR /app

# Keep .output as a subdirectory — Nitro resolves public assets relative
# to its own bundle path, so flattening the structure breaks static serving.
COPY --from=build /app/.output ./.output

# Nitro resolves public assets as '../public/' relative to nitro.mjs in
# chunks/nitro/ → lands at chunks/public/ instead of .output/public/.
# Symlink bridges the gap without duplicating files.
RUN ln -sf /app/.output/public /app/.output/server/chunks/public

EXPOSE 3000
ENV HOST=0.0.0.0 PORT=3000

# Override API base at runtime without rebuilding
ENV NUXT_PUBLIC_API_BASE=http://localhost:5000

CMD ["node", ".output/server/index.mjs"]
