class CopyInput extends HTMLElement {
  constructor() {
    super();
    const template = `
      <style>
        :host {
            display: block;
            width: 100%;
            max-width: 400px;
            margin: 0 auto;
            font-family: sans-serif;
        }
        input {
            box-sizing: border-box;
            width: 100%;
            padding: 8px;
            font-size: 16px;
            border: none;
            background-color: #f9f9f9;
        }

        button {
            margin-top: 8px;
            padding: 8px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
    </style>
    <div>
        <input type="text" readonly class="form-control">
        <button class="btn btn-primary">Copy</button>
    </div>
    `;
    // Create a shadow root for the component
    this.attachShadow({ mode: "open" });
    // Attach template to the shadow root
    this.shadowRoot.innerHTML = template;
    // Get a reference to the input and button elements
    this.inputElement = this.shadowRoot.querySelector("input");
    this.buttonElement = this.shadowRoot.querySelector("button");

    // Attach a click event listener to the button
    this.buttonElement.addEventListener("click", () => {
      // Copy the input value to the clipboard
      alert("copied: " + this.inputElement.value);
    });
  }

  // Define a getter and setter for the input value
  get value() {
    return this.inputElement.value;
  }

  set value(newValue) {
    this.inputElement.value = newValue;
  }
}
// Define the custom element
customElements.define("copy-input", CopyInput);
