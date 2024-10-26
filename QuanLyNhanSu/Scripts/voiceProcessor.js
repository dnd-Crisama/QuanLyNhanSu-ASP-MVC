// voiceProcessor.js
class VoiceProcessor extends AudioWorkletProcessor {
    constructor() {
        super();
        this.bufferSize = 2048;  // Smaller buffer for lower latency
        this.buffer = new Float32Array(this.bufferSize);
        this.bytesRecorded = 0;
    }

    process(inputs, outputs) {
        const input = inputs[0];
        if (input && input.length > 0) {
            const inputChannel = input[0];

            // Process in smaller chunks for better real-time performance
            if (this.bytesRecorded + inputChannel.length >= this.bufferSize) {
                this.port.postMessage(this.buffer.slice(0, this.bytesRecorded));
                this.buffer = new Float32Array(this.bufferSize);
                this.bytesRecorded = 0;
            }

            // Copy new samples to buffer
            for (let i = 0; i < inputChannel.length; i++) {
                this.buffer[this.bytesRecorded++] = inputChannel[i];
            }
        }
        return true;
    }
}

registerProcessor('voice-processor', VoiceProcessor);