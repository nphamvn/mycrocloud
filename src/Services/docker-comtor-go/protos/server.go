package protos

import (
	context "context"
	"log"
)

type Server struct {
	UnimplementedDockerServiceServer
}

func (s *Server) BuildImage(ctx context.Context, in *BuildImageRequest) (*BuildImageResponse, error) {
	log.Println("New message received")
	return &BuildImageResponse{Message: "Image name:" + in.GetImageName()}, nil
}
func (s *Server) RunContainer(ctx context.Context, in *RunContainerRequest) (*RunContainerResponse, error) {
	return &RunContainerResponse{Message: "Image name:" + in.GetMessage()}, nil
}
func (s *Server) StopContainer(ctx context.Context, in *StopContainerRequest) (*StopContainerResponse, error) {
	return &StopContainerResponse{Message: "Image name:" + in.GetMessage()}, nil
}
func (s *Server) RemoveImage(ctx context.Context, in *RemoveImageRequest) (*RemoveImageResponse, error) {
	return &RemoveImageResponse{Message: "Image name:" + in.GetMessage()}, nil
}
