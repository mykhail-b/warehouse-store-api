import http from 'k6/http';
import { sleep, group, check } from 'k6';

const BASE_URL = 'http://localhost:5111/api/order';

export const options = {
    stages: [
        { duration: '30s', target: 20 },
    ],
};

export default function () {
    const params = { headers: { 'Content-Type': 'application/json' } };

    group('Create Guest Order', () => {
        const payload = JSON.stringify({
            customerName: 'Guest Buyer',
            customerEmail: 'guest@example.com',
            shippingAddress: 'Test, street 123',
            items: [
                {
                    warehouseItemId: 1,
                    quantity: 1,
                    price: 99.99
                }
            ],
        });

        const res = http.post(`${BASE_URL}`, payload, params);

        check(res, {
            'status is 200': (r) => r.status === 200,
            'response contains id': (r) => r.json('id') > 0,
        });
    });

    sleep(1);
}