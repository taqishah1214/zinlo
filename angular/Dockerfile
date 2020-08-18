FROM node:13-alpine as Builder

ARG APP_API_URL
ENV APP_API_URL $APP_API_URL

WORKDIR /app/

COPY package*.json ./
COPY yarn.lock ./

RUN yarn install

COPY . .

RUN yarn run build

######################################################################
FROM nginx:alpine

COPY --from=Builder /app/dist /usr/share/nginx/html/
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

COPY . /usr/share/nginx/html
