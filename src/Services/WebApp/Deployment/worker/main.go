package main

import (
	"log"
	"sync"
	"time"

	"github.com/streadway/amqp"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}

// MaxConcurrentJobs is the limit for concurrent jobs being processed
const MaxConcurrentJobs = 3

// ProcessJob simulates job processing asynchronously
func ProcessJob(job string, wg *sync.WaitGroup) {
	defer wg.Done()

	// Simulate job processing time (5 seconds)
	log.Printf("Processing %s...", job)
	time.Sleep(5 * time.Second)
	log.Printf("Finished processing %s", job)
}

func main() {
	// Connect to RabbitMQ server
	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	// Open a channel
	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	// Declare the queue from which jobs are consumed
	q, err := ch.QueueDeclare(
		"job_queue", // name
		true,        // durable
		false,       // delete when unused
		false,       // exclusive
		false,       // no-wait
		nil,         // arguments
	)
	failOnError(err, "Failed to declare a queue")

	// Create a channel to limit the number of concurrent jobs
	jobLimit := make(chan struct{}, MaxConcurrentJobs)
	wg := &sync.WaitGroup{}

	// RabbitMQ Consumer setup
	msgs, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		true,   // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")

	forever := make(chan bool)

	go func() {
		for d := range msgs {
			job := string(d.Body)

			// Limit concurrency by using a buffered channel
			jobLimit <- struct{}{}
			wg.Add(1)

			go func(job string) {
				defer func() { <-jobLimit }() // Release the slot once the job is done
				ProcessJob(job, wg)
			}(job)
		}
	}()

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")
	<-forever

	wg.Wait() // Wait for all goroutines to finish
}
