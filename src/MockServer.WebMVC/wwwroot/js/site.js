//clipboard-copy
class ClipboardCopyElement extends HTMLElement {
  //Invoked when the component is first initialized.
  //Must call super() and can set any defaults or perform other pre-rendering processes.

  // Always call super first in constructor
  constructor() {
    console.log("ClipboardCopy element intialized");
    super();
    this.attachShadow({ mode: "open" });
  }
  //Invoked when the custom element is first connected to the document's DOM.
  connectedCallback() {
    console.log("ClipboardCopy element added to page.");
    this.text = this.getAttribute("text");
    console.log("text attribute value in connectedCallback: " + this.text);
    this.render(this.text);
  }
  render(text) {
    const button = document.createElement("button");
    button.addEventListener("click", function () {
      navigator.clipboard.writeText(text);
      button.innerText = "Copied";
    });
    button.innerHTML = '<i class="bi bi-clipboard"></i>';
    this.shadowRoot.appendChild(button);
  }
  //Returns an array of attributes that the browser will observe.
  static get observedAttributes() {
    return ["text"];
  }

  //Invoked when an attribute is defined in the HTML or changed using JavaScript.
  attributeChangedCallback(property, oldValue, newValue) {
    console.log("ClipboardCopy element attributes changed.");
    this.text = this.getAttribute("text");
    console.log(
      "text attribute value in attributeChangedCallback: " + this.text
    );
    if (oldValue === newValue) return;
    this[property] = newValue;
  }
  //
  disconnectedCallback() {
    this.text = this.getAttribute("text");
    console.log("text attribute value in disconnectedCallback: " + this.text);
    console.log("ClipboardCopy element removed from page.");
  }

  adoptedCallback() {
    this.text = this.getAttribute("text");
    console.log("text attribute value in disconnectedCallback: " + this.text);
    console.log("ClipboardCopy element moved to page.");
  }
}
// Define the new element
customElements.define("clipboard-copy", ClipboardCopyElement);

// Create a class for the element
class PopUpInfo extends HTMLElement {
  constructor() {
    // Always call super first in constructor
    super();

    // Create a shadow root
    const shadow = this.attachShadow({ mode: "open" });

    // Create spans
    const wrapper = document.createElement("span");
    wrapper.setAttribute("class", "wrapper");

    const icon = document.createElement("span");
    icon.setAttribute("class", "icon");
    icon.setAttribute("tabindex", 0);

    const info = document.createElement("span");
    info.setAttribute("class", "info");

    // Take attribute content and put it inside the info span
    const text = this.getAttribute("data-text");
    info.textContent = text;

    // Insert icon
    const img = document.createElement("img");
    img.src = this.hasAttribute("img")
      ? this.getAttribute("img")
      : "img/default.png";
    icon.appendChild(img);

    // Create some CSS to apply to the shadow dom
    const style = document.createElement("style");
    console.log(style.isConnected);

    style.textContent = `
      .wrapper {
        position: relative;
      }
      .info {
        font-size: 0.8rem;
        width: 200px;
        display: inline-block;
        border: 1px solid black;
        padding: 10px;
        background: white;
        border-radius: 10px;
        opacity: 0;
        transition: 0.6s all;
        position: absolute;
        bottom: 20px;
        left: 10px;
        z-index: 3;
      }
      img {
        width: 1.2rem;
      }
      .icon:hover + .info, .icon:focus + .info {
        opacity: 1;
      }
    `;

    // Attach the created elements to the shadow dom
    shadow.appendChild(style);
    console.log(style.isConnected);
    shadow.appendChild(wrapper);
    wrapper.appendChild(icon);
    wrapper.appendChild(info);
  }
}

// Define the new element
//customElements.define("popup-info", PopUpInfo);
