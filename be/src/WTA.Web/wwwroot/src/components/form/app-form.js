import html from '../../utils/index.js';
import appFormItem from './app-form-item.js';
import { ref, reactive, watch } from 'vue';
import { cloneDeep } from '../../utils/index.js';
import request from '../../request/index.js';

const template = html`
  <el-card class="box-card">
    <template #header>
      <div class="card-header">
        <slot name="header">
          <template v-if="model.schema?.title"> {{ model.schema?.title }} </template>
        </slot>
      </div>
    </template>
    <el-form
      ref="formRef"
      v-loading="loading"
      :inline="mode==='query'"
      :model="model.data"
      label-width="120px"
    >
      <app-form-item
        :mode="mode"
        v-model="model.data"
        :schema="model.schema"
        :errors="model.errors"
      />
      <slot name="footer">
        <el-form-item>
          <el-button
            :disabled="loading"
            type="primary"
            @click="submit"
          >
            {{$t('confirm')}}
          </el-button>
          <el-button @click="reset">{{$t('reset')}}</el-button>
        </el-form-item>
      </slot>
    </el-form>
  </el-card>
`;
export default {
  template,
  components: { appFormItem },
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
    mode: {
      type: String,
      default: 'create',
    },
  },
  emits: ['update:modelValue', 'before', 'after'],
  setup(props, context) {
    const model = reactive(props.modelValue);
    watch(model, (value) => {
      context.emit('update:modelValue', value);
    });
    //context.expose({ test: () => console.log('test') });
    //
    const formRef = ref(null);
    const loading = ref(false);
    //
    const reset = () => {
      formRef.value.resetFields();
    };
    //
    const validate = async () => {
      return formRef.value.validate();
    };
    //
    async function load(data) {
      const url = props.modelValue.url;
      const method = props.modelValue.mode === 'query' ? 'get' : 'post';
      const config = {
        url,
        method,
      };
      if (method === 'get') {
        config.params = data;
      } else {
        config.data = data;
      }
      try {
        const response = await request.request(config);
        const result = response.data?.data ?? response.data;
        return result;
      } catch (e) {
        model.errors.value = e.response.data.data ?? e.response.data;
        throw e;
      }
    }
    //
    const submit = async () => {
      try {
        const valid = await validate();
        if (valid) {
          loading.value = true;
          const data = cloneDeep(model.data);
          context.emit('before', (val) => {
            Object.assign(data, val);
          });
          const result = await load(data);
          context.emit('after', result);
        }
      } catch (error) {
        console.error(error);
      } finally {
        loading.value = false;
      }
    };
    return {
      model,
      formRef,
      loading,
      reset,
      submit,
    };
  },
};
