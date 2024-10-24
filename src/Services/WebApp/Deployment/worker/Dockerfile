FROM golang AS builder

WORKDIR /app

COPY go.mod go.sum ./

RUN go mod download

COPY . .

RUN CGO_ENABLED=0 GOOS=linux go build -o worker .

FROM node:20-alpine

WORKDIR /root/

COPY --from=builder /app/worker .

RUN apk update && apk add git

CMD ["./worker"]