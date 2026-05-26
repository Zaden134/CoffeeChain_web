# frontend.Dockerfile build Angular app roi phuc vu bang nginx.
FROM node:24-alpine AS build
WORKDIR /app

COPY src/frontend/coffee-chain-admin/package*.json ./
RUN npm install

COPY src/frontend/coffee-chain-admin/ ./
RUN npm run build

FROM nginx:1.29-alpine AS final
COPY --from=build /app/dist/coffee-chain-admin/browser /usr/share/nginx/html
EXPOSE 80
