package main

import (
	"docker-comtor/protos"
	"log"
	"net"

	"google.golang.org/grpc"
)

func main() {
	println("gRPC server tutorial in Go")
	lis, err := net.Listen("tcp", ":9000")
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
		panic(err)
	}
	s := grpc.NewServer()
	protos.RegisterDockerServiceServer(s, &protos.Server{})
	if err := s.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}
