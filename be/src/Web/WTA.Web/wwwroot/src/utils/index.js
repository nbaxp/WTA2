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
// cloneDeep
function cloneDeep(objects) {
  return _.cloneDeep(objects);
}
// format %
function persentFormat(number) {
  return `${parseFloat(number * 100).toFixed(2)} %`;
}
// format bytes
function bytesFormat(bytes) {
  if (isNaN(bytes)) {
    return '';
  }
  var symbols = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
  var exp = Math.floor(Math.log(bytes) / Math.log(2));
  if (exp < 1) {
    exp = 0;
  }
  var i = Math.floor(exp / 10);
  bytes = bytes / Math.pow(2, 10 * i);

  if (bytes.toString().length > bytes.toFixed(2).toString().length) {
    bytes = bytes.toFixed(2);
  }
  return bytes + ' ' + symbols[i];
}
// string format
function format(template, ...args) {
  const formatRegExp = /%[sdj%]/g;
  let counter = 0;
  return template.replace(formatRegExp, (match) => {
    const index = counter;
    counter += 1;
    if (match === '%%') {
      return '%';
    }
    if (index > args.length - 1) {
      return match;
    }
    if (match === '%s') {
      return String(args[index]);
    }
    if (match === '%d') {
      return Number(args[index]);
    }
    if (match === '%j') {
      return JSON.stringify(args[index]);
    }
    return match;
  });
}
// form validators
const validators = {
  compare(rule, value, callback, source, options) {
    const errors = [];
    if (value && value !== rule.data[rule.compare]) {
      const message = format(options.messages.compare, rule.title, rule.schema.properties[rule.compare].title);
      errors.push(new Error(message));
    }
    callback(errors);
  },
  true(rule, value, callback, source, options) {
    const errors = [];
    if (!value) {
      const message = format(options.messages.true, rule.title);
      errors.push(new Error(message));
    }
    callback(errors);
  },
  remote(rule, value, callback, source, options) {
    const errors = [];
    const message = format(options.messages.remote, rule.title);
    if (!value) {
      callback(errors);
    } else {
      const config = {
        url: rule.url,
        method: rule.method ?? 'get',
      };
      const data = { [rule.field]: value };
      if (config.method === 'get') {
        config.params = data;
      } else {
        config.data = data;
      }
      request
        .request(config)
        .then((response) => {
          if (response.status === 200) {
            if (response.data.code) {
              if (response.data.code !== 200) {
                errors.push(new Error(1 + response.data.message));
              }
            }
          } else {
            errors.push(new Error(2 + response.data));
          }
          callback(errors);
        })
        .catch((o) => {
          errors.push(o.response?.data?.message ?? message ?? o.message);
          callback(errors);
        });
    }
  },
};
// create rules
function createRules(parentSchema, property, data) {
  if (!property.rules) {
    return null;
  }
  const rules = [...(Array.isArray(property.rules) ? property.rules : [property.rules])].map((o) =>
    JSON.parse(JSON.stringify(o)),
  );
  Object.values(rules).forEach((rule) => {
    rule.data = data;
    rule.schema = parentSchema;
    rule.title = rule.title ?? property.title;
    rule.type = property.type;
    if (rule.validator) {
      rule.validator = validators[rule.validator];
    }
    if (!rule.message) {
      if (rule.required) {
        rule.message = format(schema.messages.required, property.title);
      } else if (rule.pattern) {
        rule.message = format(schema.messages.pattern, property.title);
      } else if (property.type === 'string' || property.type === 'number' || property.type === 'array') {
        if (rule.len) {
          rule.message = format(schema.messages[property.type].len, property.title, rule.len);
        } else if (rule.min) {
          rule.message = format(schema.messages[property.type].min, property.title, rule.min);
        } else if (rule.max) {
          rule.message = format(schema.messages[property.type].max, property.title, rule.max);
        } else if (rule.range) {
          rule.message = format(schema.messages[property.type].range, property.title, rule.range);
        }
      }
    }
  });
  return rules;
}

export default html;
export { cloneDeep, persentFormat, bytesFormat, createRules };
