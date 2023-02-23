// format html`...` by vscode lit-html
function html(strings, ...values) {
  let output = '';
  let index;
  for (index = 0; index < values.length; index += 1) {
    output += strings[index] + values[index];
  }
  output += strings[index];
  return output;
}
export default html;
