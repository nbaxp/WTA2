import html from '../utils/index.js';
import { ref, reactive, watch, onMounted } from 'vue';
import appForm from './app-form.js';

const template = html`
  <el-card>
    <el-row>
      <el-col>
        <app-form
          ref="queryFormRef"
          v-model="queryModel"
          mode="query"
          hideHeader="true"
          @before="beforeQuery"
          @after="afterQuery"
        >
          <template #footer>
            <div></div>
          </template>
        </app-form>
      </el-col>
    </el-row>
    <el-row>
      <el-col>
        <div class="d-flex justify-content-between" style="margin-bottom:18px;">
          <div>
            <el-button
              type="primary"
              @click="queryFormRef.submit()"
              >{{$t('confirm')}}</el-button
            >
            <el-button
              type="primary"
              @click="queryFormRef.reset()"
              >{{$t('reset')}}</el-button
            >
          </div>
          <div>
          <template>
          </template>
          </div>
        </div>
      </el-col>
    </el-row>
    <el-row>
      <el-col>
        <el-scrollbar>
          <el-table
            :ref="tableRef"
            row-key="id"
            border
            highlight-current-row
            table-layout="auto"
            :data="model.items"
            :default-sort="sortModel"
            :lazy="true"
            :load="lazyLoad"
            @sort-change="sortChange"
            @selection-change="onSelectionChange"
          >
            <el-table-column
              type="selection"
              align="center"
              fixed="left"
            />
            <el-table-column
              :label="$t('rowIndex')"
              type="index"
              align="center"
              fixed="left"
            >
              <template #default="scope">
                {{ (model.pageIndex - 1) * model.pageSize + scope.$index + 1 }}
              </template>
            </el-table-column>
            <template
              v-for="(item, key) in schema.properties.items.items.properties"
              :key="key"
            >
              <template v-if="item.template !== 'hiddenInput'">
                <el-table-column
                  :key="key"
                  :prop="key"
                  :label="item.title ?? key"
                  :sortable="item.sortable ? (model.disablePagination ? true : 'custom') : null"
                >
                  <template #default="scope"> {{scope.row[key]}} </template>
                </el-table-column>
              </template>
            </template>
            <slot name="operations">
              <el-table-column
                fixed="right"
                :label="$t('operations')"
              >
                <template #default="{ row }"> </template>
              </el-table-column>
            </slot>
          </el-table>
        </el-scrollbar>
      </el-col>
    </el-row>
    <el-row class="mt-4">
      <el-col>
        <el-pagination
          v-model:currentPage="model.pageIndex"
          v-model:page-size="model.pageSize"
          class="justify-content-sm-end"
          :background="true"
          layout="total, sizes, prev, pager, next, jumper"
          :total="model.totalCount"
          @size-change="onPageSizeChange"
          @current-change="onPageIndexChange"
        />
      </el-col>
    </el-row>
  </el-card>
`;

export default {
  components: { appForm },
  template,
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
  },
  emits: ['update:modelValue', 'before', 'after'],
  setup(props, context) {
    const model = reactive(props.modelValue.model);
    const schema = props.modelValue.schema;
    //
    const queryFormRef = ref(null);
    const queryModel = reactive({
      url: props.modelValue.url,
      model: props.modelValue.model.query,
      schema: props.modelValue.schema.properties.query,
    });
    const beforeQuery = (callback) => {
      callback(o => {
        return {
          pageIndex: model.pageIndex,
          pageSize: model.pageSize,
          query: o
        };
      });
    };
    const tableRef = ref(null);
    //
    return {
      model,
      schema,
      queryFormRef,
      queryModel,
      tableRef,
      beforeQuery
    };
  },
};
