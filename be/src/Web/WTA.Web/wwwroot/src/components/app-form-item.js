import html from '../utils/index.js';
import { ref, reactive, watch } from 'vue';
import appFormInput from './app-form-input.js';
import { createRules } from '../utils/index.js';

const template = html` <template
  v-for="(value, key) in schema.properties"
  :key="key"
>
  <template v-if="value.type==='object'"> 对象</template>
  <template v-else-if="value.type==='array'"> 数组</template>
  <template v-else>
    <template v-if="value.template!=='hiddenInput'">
      <el-form-item
        :prop="getProp(key)"
        :label="value.title"
        :rules="mode==='query' ?[]: getRules(value)"
      >
        <app-form-input
          :mode="mode"
          :prop="key"
          v-model="model"
          :schema="value"
        />
      </el-form-item>
    </template>
  </template>
</template>`;
export default {
  components: { appFormInput },
  template,
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
    mode: {
      type: String,
      default: 'create',
    },
    schema: {
      type: Object,
      default: null,
    },
    erros: {
      type: Object,
      default: null,
    },
    prefix: {
      type: String,
      default: null,
    },
  },
  emits: ['update:modelValue', 'before', 'after'],
  setup(props, context) {
    const model = reactive(props.modelValue);
    watch(model, (value) => {
      context.emit('update:modelValue', value);
    });
    //
    const getProp = (key) => {
      return props.prefix ? `${props.prefix}.${key}` : key;
    };
    //
    const getRules = (property) => {
      return createRules(props.schema, property, model);
    };
    return {
      model,
      getProp,
      getRules,
    };
  },
};
