import pymssql
import yaml
import datetime
import telebot
from telebot import types


with open('config.yaml', 'r') as config_file:
    config = yaml.load(config_file, yaml.SafeLoader)
bot = telebot.TeleBot(config['bot']['api'])
payment_provider = config['payment']['ukassa']

start_menu = types.ReplyKeyboardMarkup(resize_keyboard=True)
log_in_button = types.KeyboardButton('Войти в аккаунт')
info_button = types.KeyboardButton('О нашем компьютерном клубе')
start_menu.row(log_in_button)
start_menu.row(info_button)

main_menu = types.ReplyKeyboardMarkup(resize_keyboard=True)
profile_button = types.KeyboardButton('Мой профиль')
reservation_button = types.KeyboardButton('Мои резервации')
top_up_button = types.KeyboardButton('Пополнить баланс')
log_out_button = types.KeyboardButton('Выйти из аккаунта')
main_menu.row(profile_button)
main_menu.row(reservation_button)
main_menu.row(top_up_button)
main_menu.row(log_out_button)

back_menu = types.ReplyKeyboardMarkup(resize_keyboard=True)
back_to_previous_menu_button = types.KeyboardButton('Назад')
back_menu.row(back_to_previous_menu_button)

reservations_menu = types.ReplyKeyboardMarkup(resize_keyboard=True)
upcoming_reservations_button = types.KeyboardButton('Предстоящие')
past_reservations_button = types.KeyboardButton('Прошедшие')
reservations_menu.row(upcoming_reservations_button)
reservations_menu.row(past_reservations_button)
reservations_menu.row(back_to_previous_menu_button)

inline_cancel_reservation_buttons = types.InlineKeyboardMarkup()
yes_cancel_reservation_button = types.InlineKeyboardButton('Да', callback_data='yes_cancel_reservation')
no_cancel_reservation_button = types.InlineKeyboardButton('Нет', callback_data='no_cancel_reservation')
inline_cancel_reservation_buttons .add(yes_cancel_reservation_button, no_cancel_reservation_button)


def get_value_telegram_user(telegram_user_id, attribute):
    conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                           database='OasisDB_Telegram')
    cursor = conn.cursor()
    query = "SELECT " + attribute + " FROM Users WHERE Telegram_id={0}".format(telegram_user_id)
    try:
        cursor.execute(query)
        data = cursor.fetchone()
    except pymssql.Error as e:
        print(e)
        return
    finally:
        conn.close()

    return data[0]


def set_value_telegram_user(telegram_user_id, attribute, value):
    conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                           database='OasisDB_Telegram')
    cursor = conn.cursor()
    if attribute == 'Login' and value != 'NULL':
        query = "UPDATE Users SET " + attribute + "='{0}' WHERE Telegram_id={1}".format(value, telegram_user_id)
    else:
        query = "UPDATE Users SET " + attribute + "={0} WHERE Telegram_id={1}".format(value, telegram_user_id)
    try:
        cursor.execute(query)
        conn.commit()
    except pymssql.Error as e:
        print(e)
        return
    finally:
        conn.close()


def user_password_verify(message):
    if message.text == 'Назад':
        set_value_telegram_user(message.chat.id, 'Login', 'NULL')
        bot.send_message(message.chat.id, 'Попытка входа в аккаунт отменена', reply_markup=start_menu)
    else:
        conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                               database='OasisDB')
        cursor = conn.cursor()
        entered_password = message.text
        intended_login = get_value_telegram_user(message.chat.id, 'Login')
        find_wpf_user_password_by_login_query = "SELECT Password FROM People WHERE Login='{0}'".format(intended_login)
        try:
            cursor.execute(find_wpf_user_password_by_login_query)
            real_password = cursor.fetchone()[0]
            if real_password == entered_password:
                bot.send_message(message.chat.id, 'Вход выполнен успешно', reply_markup=main_menu)
                set_value_telegram_user(message.chat.id, 'Logged_in', '1')
            else:
                msg = bot.send_message(message.chat.id, 'Неправильный пароль', reply_markup=back_menu)
                bot.register_next_step_handler(msg, user_password_verify)
        except pymssql.Error as e:
            print(e)
            bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка', reply_markup=start_menu)
            return
        finally:
            conn.close()


def user_existence_check(message):
    if message.text == 'Назад':
        bot.send_message(message.chat.id, 'Попытка входа в аккаунт отменена', reply_markup=start_menu)
    else:
        identifier = message.text
        user_exists = False
        conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                               database='OasisDB')
        cursor = conn.cursor()
        find_wpf_user_by_login_query = "SELECT Login FROM People WHERE Login='{0}'".format(identifier)
        try:
            cursor.execute(find_wpf_user_by_login_query)
            user = cursor.fetchone()
            if user is not None:
                user_exists = True
            else:
                find_wpf_user_by_email_query = "SELECT Login FROM People WHERE Email='{0}'".format(identifier)
                cursor.execute(find_wpf_user_by_email_query)
                user = cursor.fetchone()
                if user is not None:
                    user_exists = True
            if user_exists:
                set_value_telegram_user(message.chat.id, 'Login', user[0])
                msg = bot.send_message(message.chat.id, 'Введите пароль', reply_markup=back_menu)
                bot.register_next_step_handler(msg, user_password_verify)
            else:
                msg = bot.send_message(message.chat.id, 'Данный пользователь не найден', reply_markup=back_menu)
                bot.register_next_step_handler(msg, user_existence_check)
        except pymssql.Error as e:
            print(e)
            bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка', reply_markup=start_menu)
            return
        finally:
            conn.close()


def user_profile_show(telegram_user_id):
    user_login = get_value_telegram_user(telegram_user_id, 'Login')
    conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                           database='OasisDB')
    cursor = conn.cursor()
    get_user_profile_data_query = "SELECT Email, Phone, Login, Name, Surname, Balance " \
                                  "FROM People WHERE Login='{0}'".format(user_login)
    try:
        cursor.execute(get_user_profile_data_query)
        user_profile = list(cursor.fetchone())
        for i in range(len(user_profile)):
            if user_profile[i] is None:
                user_profile[i] = 'Не указано'
            user_profile[i] = str(user_profile[i])
        bot.send_message(telegram_user_id,
                         '*Почта:* {0}\n*Номер телефона:* {1}\n*Логин:* {2}\n*Имя:* {3}\n*Фамилия:* {4}'
                         '\n\n*Текущий баланс:* {5} руб.'.format(user_profile[0], user_profile[1],
                                                                 user_profile[2], user_profile[3],
                                                                 user_profile[4], user_profile[5]),
                         reply_markup=main_menu, parse_mode='Markdown')
    except pymssql.Error as e:
        print(e)
        bot.send_message(telegram_user_id, 'На стороне сервера возникла ошибка', reply_markup=main_menu)
        return
    finally:
        conn.close()


def user_reservations_show(message):
    if message.text == 'Назад':
        bot.send_message(message.chat.id, 'Просмотр резерваций прекращен', reply_markup=main_menu)
    else:
        if message.text != upcoming_reservations_button.text and message.text != past_reservations_button.text:
            msg = bot.send_message(message.chat.id, 'Прости, но я тебя не понимаю')
            bot.register_next_step_handler(msg, user_reservations_show)
        else:
            conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                                   database='OasisDB')
            cursor = conn.cursor()
            user_login = get_value_telegram_user(message.chat.id, 'Login')
            current_date = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S.000')
            get_user_id_by_login_query = "SELECT Id From People WHERE Login='{0}'".format(user_login)
            try:
                cursor.execute(get_user_id_by_login_query)
                user_id = cursor.fetchone()[0]
                if message.text == upcoming_reservations_button.text:
                    get_needed_reservations_query \
                        = "SELECT Reservations.Id, Reservations.SeatId, Halls.Name, Reservations.StartTime," \
                          " Reservations.Hours, Reservations.Price FROM Reservations JOIN Seats on" \
                          " Reservations.SeatId = Seats.Id JOIN Halls ON Seats.HallId = Halls.Id WHERE UserId = {0} " \
                          "AND StartTime > '{1}'".format(user_id, current_date)
                else:
                    get_needed_reservations_query \
                        = "SELECT Reservations.Id, Reservations.SeatId, Halls.Name, Reservations.StartTime," \
                          " Reservations.Hours, Reservations.Price FROM Reservations JOIN Seats on" \
                          " Reservations.SeatId = Seats.Id JOIN Halls ON Seats.HallId = Halls.Id WHERE UserId = {0} " \
                          "AND StartTime < '{1}'".format(user_id, current_date)
                cursor.execute(get_needed_reservations_query)
                user_needed_reservations = list(cursor.fetchall())
                if len(user_needed_reservations) == 0:
                    bot.send_message(message.chat.id, 'Резервации с такими параметрами отсутствуют',
                                     reply_markup=main_menu)
                else:
                    user_reservations_message = ''
                    for i in range(len(user_needed_reservations)):
                        reservation_id = user_needed_reservations[i][0]
                        seat_number = user_needed_reservations[i][1]
                        hall_name = user_needed_reservations[i][2]
                        start_time = user_needed_reservations[i][3]
                        hours = user_needed_reservations[i][4]
                        price = user_needed_reservations[i][5]
                        finish_time = start_time + datetime.timedelta(hours=int(hours))
                        user_reservations_message += \
                            '*Резервация №{0}:*\nНомер места - {1}\nНазвание зала - {2}\nВремя - с {3} по {4}' \
                            '\nСтоимость - {5} руб.\n\n'.format(
                                reservation_id, seat_number, hall_name, start_time.strftime('%d.%m %H:%M'),
                                finish_time.strftime('%d.%m %H:%M'), price)
                    if len(user_reservations_message) > 4096:
                        for i in range(0, len(user_reservations_message), 4096):
                            bot.send_message(message.chat.id, user_reservations_message[i: i+4096],
                                             reply_markup=main_menu, parse_mode='Markdown')
                    else:
                        bot.send_message(message.chat.id, user_reservations_message,
                                         reply_markup=main_menu, parse_mode='Markdown')
                    if message.text == upcoming_reservations_button.text:
                        bot.send_message(message.chat.id, 'Хотите ли вы отменить какую-нибудь резервацию?',
                                         reply_markup=inline_cancel_reservation_buttons)
            except pymssql.Error as e:
                print(e)
                bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка', reply_markup=main_menu)
                return
            finally:
                conn.close()


def cancel_user_reservation(message):
    if message.text == 'Назад':
        bot.send_message(message.chat.id, 'Попытка отмены резервации прекращена', reply_markup=main_menu)
    else:
        try:
            reservation_number = int(message.text)
            if reservation_number < 0:
                msg = bot.send_message(message.chat.id, 'Номер резервации не может быть отрицательным числом',
                                       reply_markup=back_menu)
                bot.register_next_step_handler(msg, cancel_user_reservation)
            else:
                user_login = get_value_telegram_user(message.chat.id, 'Login')
                conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                                       database='OasisDB')
                cursor = conn.cursor()
                get_user_reservation = "SELECT Reservations.StartTime, Reservations.Price FROM Reservations JOIN " \
                                       "People ON Reservations.UserId = People.Id WHERE Reservations.Id={0} AND" \
                                       " People.Login='{1}'".format(reservation_number, user_login)
                try:
                    cursor.execute(get_user_reservation)
                    reservation_to_cancel = cursor.fetchone()
                    if reservation_to_cancel is None:
                        msg = bot.send_message(message.chat.id, 'У вас отсутствует резервация с данным номером',
                                               reply_markup=back_menu)
                        bot.register_next_step_handler(msg, cancel_user_reservation)
                    else:
                        reservation_to_cancel_start = reservation_to_cancel[0]
                        current_time = datetime.datetime.now()
                        if reservation_to_cancel_start > current_time:
                            reservation_to_cancel_price = int(reservation_to_cancel[1])
                            get_current_user_balance_query = "SELECT Balance FROM People WHERE " \
                                                             "Login='{0}'".format(user_login)
                            cursor.execute(get_current_user_balance_query)
                            current_balance = int(cursor.fetchone()[0])
                            new_balance = current_balance + reservation_to_cancel_price
                            update_user_balance_query = "UPDATE People SET Balance={0} WHERE " \
                                                        "Login='{1}'".format(new_balance, user_login)
                            cursor.execute(update_user_balance_query)
                            delete_reservation_query = "DELETE FROM Reservations WHERE " \
                                                       "Reservations.Id = {0}".format(reservation_number)
                            cursor.execute(delete_reservation_query)
                            conn.commit()
                            bot.send_message(message.chat.id, 'Успешно отменено, деньги возвращены на ваш баланс',
                                             reply_markup=main_menu)
                        else:
                            msg = bot.send_message(message.chat.id, 'Отменить можно только не начавшуюся резервацию',
                                                   reply_markup=back_menu)
                            bot.register_next_step_handler(msg, cancel_user_reservation)
                except pymssql.Error as e:
                    print(e)
                    bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка', reply_markup=main_menu)
                    return
                finally:
                    conn.close()
        except ValueError:
            msg = bot.send_message(message.chat.id, 'Необходимо ввести число', reply_markup=back_menu)
            bot.register_next_step_handler(msg, cancel_user_reservation)


@bot.callback_query_handler(func=lambda call: True)
def inline_buttons_answer(call):
    if call.data == 'yes_cancel_reservation':
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id,
                              text='Хотите ли вы отменить какую-нибудь резервацию?')
        msg = bot.send_message(call.message.chat.id, 'Пожалуйста, введите номер резервации из сообщения выше',
                               reply_markup=back_menu)
        bot.register_next_step_handler(msg, cancel_user_reservation)
    elif call.data == 'no_cancel_reservation':
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id,
                              text='Хотите ли вы отменить какую-нибудь резервацию?')


def user_top_up_balance(message):
    if message.text == 'Назад':
        bot.send_message(message.chat.id, 'Попытка пополнения баланса отменена', reply_markup=main_menu)
    else:
        try:
            replenishment = int(message.text)
            if replenishment <= 0:
                msg = bot.send_message(message.chat.id, 'Пополнение баланса возможно только на положительную сумму',
                                       reply_markup=back_menu)
                bot.register_next_step_handler(msg, user_top_up_balance)
            elif 0 < replenishment < 100:
                msg = bot.send_message(message.chat.id, 'Минимальная сумма пополнения составляет 100 руб.',
                                       reply_markup=back_menu)
                bot.register_next_step_handler(msg, user_top_up_balance)
            else:
                price = [types.LabeledPrice(label='К оплате', amount=replenishment * 100)]
                bot.send_invoice(
                    message.chat.id,
                    'Пополнение баланса',
                    'Не торопитесь. Вы сможете оплатить покупку в любой удобный для вас момент',
                    get_value_telegram_user(message.chat.id, 'Login') + '-Пополнение',
                    payment_provider,
                    'RUB',
                    price,
                    photo_url='https://s320sas.storage.yandex.net/rdisk/ac9ca4f63a978a879b9970aa19baa03a31a0ebf90ec3fbd6b337b399479d566f/62ba43e8/tFFGxA3Iovy1caSk5i2AvceUI7RCAnHkHkQNbydmp2qc2AL1dsraUI5EmOvk8_wA6OplSK5ZN6thqLEUhxxfyw==?uid=0&filename=logo.jpg&disposition=inline&hash=&limit=0&content_type=image%2Fjpeg&owner_uid=0&fsize=139757&hid=af3a24997015cc106e2371b678ad3c43&media_type=image&tknv=v2&etag=8c7248acc342620bcd9bd914ce98f46d&rtoken=o9wIPgWxYPFA&force_default=no&ycrid=na-95ecc0e31442e141aa2e7ff87694d5d1-downloader1h&ts=5e276aeaaca00&s=11410af574f249fe4ac8fe7cdc9874a96df09e5bd2317b3e7fe7f7dcbfdc1ded&pb=U2FsdGVkX18713f0Qp0jwFm1vbL4aJo9db7qoxp0JeLMZPpUB4HdgeaHIBR08NYmbDDF1Ua47SHZiLcXwCtXX4UPhEkU3_iJBCoWp0NQkxw',
                    photo_height=256,
                    photo_width=256,
                    photo_size=256,
                    is_flexible=False,
                    start_parameter='oasis_top_up_balance')
                bot.send_message(message.chat.id, 'Мы используем платежную систему *ЮKassa*, '
                                                  'ваши данные находятся в безопасности и не передаются третьим лицам',
                                 reply_markup=main_menu, parse_mode='Markdown')
        except ValueError:
            msg = bot.send_message(message.chat.id, 'Необходимо ввести число', reply_markup=back_menu)
            bot.register_next_step_handler(msg, user_top_up_balance)


@bot.pre_checkout_query_handler(func=lambda query: True)
def checkout(pre_checkout_query):
    bot.answer_pre_checkout_query(
        pre_checkout_query.id, ok=True,
        error_message="К сожалению оплата не прошла. Попробуйте еще раз или обратитесь в поддержку по телефону.")


@bot.message_handler(content_types=['successful_payment'])
def got_payment(message):
    user_login = get_value_telegram_user(message.chat.id, 'Login')
    replenishment = message.successful_payment.total_amount / 100
    conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                           database='OasisDB')
    cursor = conn.cursor()
    get_current_user_balance_query = "SELECT Balance FROM People WHERE Login='{0}'".format(user_login)
    try:
        cursor.execute(get_current_user_balance_query)
        current_balance = int(cursor.fetchone()[0])
        new_balance = current_balance + replenishment
        update_user_balance_query = "UPDATE People SET Balance={0} WHERE Login='{1}'".format(new_balance, user_login)
        cursor.execute(update_user_balance_query)
        conn.commit()
    except pymssql.Error as e:
        print(e)
        bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка. '
                                          'Обратитесь к администратору и вам вернут деньги', reply_markup=main_menu)
        return
    finally:
        conn.close()
    bot.send_message(
        message.chat.id, 'Большое спасибо за покупку в нашем компьютерном клубе. '
                         'Ваш баланс был увеличен на `{0} {1}` \n'
                         'Отличной игры и больших побед!'.format(replenishment, message.successful_payment.currency),
        reply_markup=main_menu, parse_mode='Markdown')


def user_log_out(telegram_user_id):
    set_value_telegram_user(telegram_user_id, 'Login', 'NULL')
    set_value_telegram_user(telegram_user_id, 'Logged_in', 0)


@bot.message_handler(commands=['start'])
def start_message(message):
    conn = pymssql.connect(server='vm-as35.staff.corp.local', user='student', password='sql2020',
                           database='OasisDB_Telegram')
    cursor = conn.cursor()
    check_previous_communication_query = "SELECT * FROM Users WHERE Telegram_id={0}".format(message.chat.id)
    try:
        cursor.execute(check_previous_communication_query)
        telegram_user = cursor.fetchone()
        if telegram_user is None:
            add_new_telegram_user_query = "INSERT INTO Users (Telegram_id) VALUES ({0})".format(message.chat.id)
            cursor.execute(add_new_telegram_user_query)
            conn.commit()
    except pymssql.Error as e:
        print(e)
        bot.send_message(message.chat.id, 'На стороне сервера возникла ошибка', reply_markup=start_menu)
        return
    finally:
        conn.close()
    bot.send_message(message.chat.id, 'Рад приветствовать вас!', reply_markup=start_menu)


@bot.message_handler(content_types=['text'])
def get_text_messages(message):
    is_logged = get_value_telegram_user(message.chat.id, 'Logged_in')
    if message.text == log_in_button.text:
        if is_logged:
            bot.send_message(message.chat.id, 'Вы уже находитесь в аккаунте', reply_markup=main_menu)
        else:
            msg = bot.send_message(message.chat.id, 'Введите вашу почту или логин', reply_markup=back_menu)
            bot.register_next_step_handler(msg, user_existence_check)
    elif message.text == info_button.text:
        bot.send_message(message.chat.id, '*EZ KATKA* - сеть многофункциональных киберспортивных арен, '
                                          'которая не только предоставляет возможность доступной игры для каждого, '
                                          'но и заботится о помощи молодым игрокам в данной сфере.'
                                          '\n\nДля более детального ознакомления, используйте наш сайт:'
                                          ' https://ez-katka.ru/',
                         reply_markup=start_menu, parse_mode='Markdown')
    elif message.text == profile_button.text:
        if is_logged:
            user_profile_show(message.chat.id)
        else:
            bot.send_message(message.chat.id, 'Данное действие невозможно, так как вы пока еще не вошли в аккаунт',
                             reply_markup=start_menu)
    elif message.text == reservation_button.text:
        if is_logged:
            msg = bot.send_message(message.chat.id, 'Выберите опцию', reply_markup=reservations_menu)
            bot.register_next_step_handler(msg, user_reservations_show)
        else:
            bot.send_message(message.chat.id, 'Данное действие невозможно, так как вы пока еще не вошли в аккаунт',
                             reply_markup=start_menu)
    elif message.text == top_up_button.text:
        if is_logged:
            msg = bot.send_message(message.chat.id, 'Введите желаймую сумму пополнения в рублях',
                                   reply_markup=back_menu)
            bot.register_next_step_handler(msg, user_top_up_balance)
        else:
            bot.send_message(message.chat.id, 'Данное действие невозможно, так как вы пока еще не вошли в аккаунт',
                             reply_markup=start_menu)
    elif message.text == log_out_button.text:
        if not is_logged:
            bot.send_message(message.chat.id, 'Данное действие невозможно, так как вы пока еще не вошли в аккаунт',
                             reply_markup=start_menu)
        else:
            bot.send_message(message.chat.id, 'Выход из аккаунта выполнен', reply_markup=start_menu)
            user_log_out(message.chat.id)
    else:
        bot.send_message(message.chat.id, 'Прости, но я тебя не понимаю')


if __name__ == '__main__':
    bot.polling(none_stop=True)
