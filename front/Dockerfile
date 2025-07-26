FROM node:20-alpine AS build

RUN apk add --no-cache \
    python3 \
    make \
    g++ \
    libc6-compat

WORKDIR /app

COPY package*.json ./

RUN npm cache clean --force && \
    npm install --force && \
    npm install @rollup/rollup-linux-x64-musl --save-optional --force

COPY . .

RUN npm run build

FROM nginx:alpine AS production

COPY --from=build /app/dist /usr/share/nginx/html

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]